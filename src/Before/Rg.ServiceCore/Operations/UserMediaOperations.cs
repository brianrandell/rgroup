using Rg.ServiceCore.DbModel;
using Rg.ApiTypes;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Rg.ServiceCore.Operations
{
    public static class UserMediaOperations
    {
        public static async Task<AlbumItem> GetMediaAsync(
            ApplicationDbContext dbContext,
            int mediaId)
        {
            UserMedia entity = await dbContext.UserMedias
                .Include(a => a.Description)
                .Include(a => a.Likes.Select(like => like.User))
                .Include(a => a.CommentThread.Comments.Select(c => c.Text))
                .Include(a => a.CommentThread.Comments.Select(c => c.User.Avatar))
                .SingleOrDefaultAsync(a => a.UserMediaId == mediaId);
            if (entity == null)
            {
                return null;
            }

            return new AlbumItem
            {
                Id = entity.UserMediaId,
                UserId = entity.UserId,
                Title = entity.Title,
                Description = entity.Description?.Content,
                CreatedTime = entity.CreatedUtc.ToString("s"),
                CreatedTimeAgo = TimeOperations.GetTimeAgo(entity.CreatedUtc),
                MediaUrl = UserMediaOperations.GetUrl(entity),
                LikeUrl = UserMediaOperations.GetLikeUrl(entity),
                LikeGroups = LikeOperations.MakeLikeGroups(entity.Likes),
                CommentUrl = UserMediaOperations.GetCommentUrl(entity),
                Comments = CommentOperations.GetComments(entity.CommentThread)
            };
        }

        public static async Task<MediaUploadResults> StoreMediaAsync(
            ApplicationDbContext dbContext,
            UserInfo userEntity,
            IEnumerable<UploadedMedia> items)
        {
            var newEntities = new List<UserMedia>();
            foreach (UploadedMedia item in items)
            {
                if (new FileInfo(item.LocalFileName).Length > 0)
                {
                    string ext = Path.GetExtension(item.OriginalName.Trim('\"')).Substring(1);
                    var entity = new UserMedia
                    {
                        User = userEntity,
                        State = UserMediaState.UploadedButUnused,
                        CreatedUtc = DateTime.UtcNow,
                        Extension = ext
                    };
                    var dataEntity = new UserMediaData
                    {
                        UserMedia = entity,
                        ImageData = File.ReadAllBytes(item.LocalFileName)
                    };
                    File.Delete(item.LocalFileName);
                    dbContext.UserMedias.Add(entity);
                    dbContext.UserMediaDatas.Add(dataEntity);

                    newEntities.Add(entity);
                }
            }

            await dbContext.SaveChangesAsync();

            await SearchOperations.IndexMediaAsync(newEntities);

            return new MediaUploadResults
            {
                Files = newEntities
                    .Select(
                        e => new MediaUploadResult
                        {
                            Id = e.UserMediaId
                        })
                    .ToList()
            };
        }

        public static string GetUrl(UserMedia entity)
        {
            return $"/api/userimages/{entity.UserMediaId}.{entity.Extension}";
        }

        public static string GetLikeUrl(UserMedia entity)
        {
            return $"/api/userimages/{entity.UserMediaId}.{entity.Extension}/Like";
        }

        public static string GetCommentUrl(UserMedia entity)
        {
            return $"/api/userimages/{entity.UserMediaId}.{entity.Extension}/Comment";
        }

        public static async Task<HttpStatusCode> AddOrRemoveMediaLikeAsync(
            ApplicationDbContext dbContext,
            string userId,
            int mediaId,
            LikeRequest like)
        {
            UserMedia mediaEntity = await dbContext.UserMedias
                .SingleOrDefaultAsync(te => te.UserMediaId == mediaId);
            if (mediaEntity == null)
            {
                // The entry id is part of the URL, so return a 404.
                return HttpStatusCode.NotFound;
            }

            return await LikeOperations.AddOrRemoveLikeAsync(
                dbContext,
                userId,
                mediaId,
                le => le.UserMediaId,
                like);
        }

        public static async Task<HttpStatusCode> AddMediaCommentAsync(
            ApplicationDbContext dbContext,
            string userId,
            int mediaId,
            CommentRequest comment)
        {
            UserMedia mediaEntity = await dbContext.UserMedias
                .SingleOrDefaultAsync(te => te.UserMediaId == mediaId);
            if (mediaEntity == null)
            {
                // The entry id is part of the URL, so return a 404.
                return HttpStatusCode.NotFound;
            }

            return await CommentOperations.AddCommentAsync(
                dbContext,
                userId,
                new CommentItemIds { MediaId = mediaId, AlbumId = mediaEntity.MediaAlbumId },
                e => e.CommentThread,
                mediaEntity,
                comment);
        }

        public class UploadedMedia
        {
            public string LocalFileName { get; set; }
            public string OriginalName { get; set; }
        }
    }
}
