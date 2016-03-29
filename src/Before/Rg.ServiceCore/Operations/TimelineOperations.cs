using Rg.ApiTypes;
using Rg.ServiceCore.DbModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

namespace Rg.ServiceCore.Operations
{
    /// <summary>
    /// Timeline operations used from both web API and UI controllers.
    /// </summary>
    public static class TimelineOperations
    {
        public static async Task<IList<TimelineEntryDetails>> GetTimelineItemsAsync(
            ApplicationDbContext dbContext,
            string since)
        {
            IQueryable<TimelineEntry> q = dbContext.TimelineEntries;
            if (!string.IsNullOrWhiteSpace(since))
            {
                DateTime after;
                if (!DateTime.TryParseExact(since, "s", CultureInfo.InvariantCulture, DateTimeStyles.None, out after))
                {
                    return null;
                }

                q = q.Where(te => te.Message.CreatedUtc > after);
            }

            q = q.OrderByDescending(te => te.Message.CreatedUtc).Take(30);

            return await EntriesQueryToDetailsAsync(q);
        }

        public static async Task<TimelineEntryDetails> GetTimelineItemAsync(
            ApplicationDbContext dbContext,
            int entryId)
        {
            IList<TimelineEntryDetails> result = await EntriesQueryToDetailsAsync(
                dbContext.TimelineEntries.Where(e => e.TimelineEntryId == entryId));
            return result.SingleOrDefault();
        }

        private static async Task<IList<TimelineEntryDetails>> EntriesQueryToDetailsAsync(
            IQueryable<TimelineEntry> q)
        {
            var results = await q
                .Include(te => te.User.Avatar)
                .Include(te => te.Message.Likes.Select(like => like.User))
                .Include(te => te.CommentThread.Comments.Select(c => c.Text))
                .Include(te => te.CommentThread.Comments.Select(c => c.User.Avatar))
                .Select(
                    te =>
                    new
                    {
                        te.TimelineEntryId,
                        User = te.User,
                        Message = te.Message.Content,
                        DateTime = te.Message.CreatedUtc,
                        Media = te.Media.Select(tme => tme.Media),
                        Likes = te.Message.Likes,
                        te.CommentThread
                    })
                .ToListAsync();
            return results.Select(
                te =>
                new TimelineEntryDetails
                {
                    UserId = te.User.UserInfoId,
                    UserName = te.User.Name,
                    AvatarUrl = UserOperations.GetAvatarUrl(te.User),
                    Message = te.Message,
                    DateTime = te.DateTime.ToString("s"),
                    MediaUrls = te.Media.Select(UserMediaOperations.GetUrl).ToList(),
                    LikeUrl = $"/api/Timeline/{te.TimelineEntryId}/Like",
                    LikeGroups = LikeOperations.MakeLikeGroups(te.Likes),
                    CommentUrl = $"/api/Timeline/{te.TimelineEntryId}/Comment",
                    Comments = CommentOperations.GetComments(te.CommentThread)
                })
                .ToList();
        }

        public static async Task<HttpStatusCode> AddTimelineEntryAsync(
            CreateTimelineEntry createMessage,
            ApplicationDbContext dbContext,
            UserInfo userEntity)
        {
            if (string.IsNullOrWhiteSpace(createMessage?.Message))
            {
                return HttpStatusCode.BadRequest;
            }

            var text = await TextOperations.CreateTextAsync(dbContext, createMessage.Message);
            var timelineEntity = new TimelineEntry
            {
                UserId = userEntity.UserInfoId,
                Message = text
            };

            if (createMessage.MediaIds != null && createMessage.MediaIds.Count > 0)
            {
                MediaAlbum timelineAlbum = await EnsureTimelinePhotoAlbumExistsAsync(dbContext, userEntity);
                timelineEntity.Media = new List<TimelineEntryMedia>();
                int sequence = 0;
                var includedMedia = new List<UserMedia>();
                foreach (int id in createMessage.MediaIds)
                {
                    UserMedia mediaEntity = await dbContext.UserMedias
                        .SingleAsync(um => um.UserMediaId == id);
                    if (mediaEntity.UserId != userEntity.UserInfoId)
                    {
                        // Only allowed to post your own images here
                        return HttpStatusCode.BadRequest;
                    }
                    includedMedia.Add(mediaEntity);
                    mediaEntity.MediaAlbum = timelineAlbum;
                    var mediaEntry =
                        new TimelineEntryMedia
                        {
                            Media = mediaEntity,
                            Sequence = sequence++,
                            TimelineEntry = timelineEntity
                        };
                    dbContext.TimelineEntryMedia.Add(mediaEntry);
                    timelineEntity.Media.Add(mediaEntry);
                }
                foreach (UserMedia media in includedMedia)
                {
                    if (media.State == UserMediaState.UploadedButUnused)
                    {
                        media.State = UserMediaState.InUse; 
                    }
                }
            }

            dbContext.UserTexts.Add(text);
            dbContext.TimelineEntries.Add(timelineEntity);
            await dbContext.SaveChangesAsync();

            await UserOperations.NotifyMentionsAsync(
                dbContext, "Timeline Entry", userEntity.UserInfoId, text);
            await SearchOperations.IndexTimelineMessageAsync(timelineEntity);

            return HttpStatusCode.OK;
        }

        public static async Task<HttpStatusCode> AddOrRemoveTimelineEntryLikeAsync(
            ApplicationDbContext dbContext,
            string userId,
            int entryId,
            LikeRequest like)
        {
            TimelineEntry entryEntity = await dbContext.TimelineEntries
                .SingleOrDefaultAsync(te => te.TimelineEntryId == entryId);
            if (entryEntity == null)
            {
                // The entry id is part of the URL, so return a 404.
                return HttpStatusCode.NotFound;
            }

            return await LikeOperations.AddOrRemoveLikeAsync(
                dbContext,
                userId,
                entryId,
                le => le.UserTextId,
                like);
        }

        public static async Task<HttpStatusCode> AddTimelineEntryCommentAsync(
            ApplicationDbContext dbContext,
            string userId,
            int entryId,
            CommentRequest comment)
        {
            TimelineEntry entryEntity = await dbContext.TimelineEntries
                .SingleOrDefaultAsync(te => te.TimelineEntryId == entryId);
            if (entryEntity == null)
            {
                // The entry id is part of the URL, so return a 404.
                return HttpStatusCode.NotFound;
            }

            return await CommentOperations.AddCommentAsync(
                dbContext,
                userId,
                new CommentItemIds {  TimelineEntryId = entryId },
                e => e.CommentThread,
                entryEntity,
                comment);
        }

        private static async Task<MediaAlbum> EnsureTimelinePhotoAlbumExistsAsync(
            ApplicationDbContext dbContext,
            UserInfo userEntity)
        {
            if (!userEntity.TimelineImagesMediaAlbumId.HasValue)
            {
                var timelineAlbum = new MediaAlbum
                {
                    User = userEntity,
                    Title = "Timeline Photos",
                    Description = "Photos posted to my timeline"
                };
                userEntity.TimelineImagesAlbum = timelineAlbum;
                dbContext.MediaAlbums.Add(timelineAlbum);

                await dbContext.SaveChangesAsync();

                return timelineAlbum;
            }

            if (userEntity.TimelineImagesAlbum == null)
            {
                await dbContext.Entry(userEntity).Reference(u => u.TimelineImagesAlbum).LoadAsync();
            }

            return userEntity.TimelineImagesAlbum;
        }
    }
}
