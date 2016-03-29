using Rg.ApiTypes;
using Rg.ServiceCore.DbModel;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

namespace Rg.ServiceCore.Operations
{
    public static class AlbumOperations
    {
        public static async Task<IList<AlbumSummary>> GetAlbumSummariesAsync(
            ApplicationDbContext dbContext,
            UserInfo user)
        {
            var results = await dbContext.MediaAlbums
                .Where(a => a.UserId == user.UserInfoId)
                .Include(a => a.Likes.Select(like => like.User))
                .Include(a => a.CommentThread.Comments.Select(c => c.Text))
                .Include(a => a.CommentThread.Comments.Select(c => c.User.Avatar))
                .Select(a =>
                    new
                    {
                        a.MediaAlbumId,
                        a.Title,
                        a.Description,
                        Medias = a.UserMedias.Take(4),
                        LatestMedia = a.UserMedias.OrderByDescending(um => um.CreatedUtc).FirstOrDefault(),
                        Likes = a.Likes,
                        a.CommentThread
                    })
                .ToListAsync();

            return results
                .Select(
                    a =>
                    new AlbumSummary
                    {
                        Id = a.MediaAlbumId,
                        UserId = user.UserInfoId,
                        Title = a.Title,
                        Description = a.Description,
                        SampleMediaUrls = a.Medias.Select(UserMediaOperations.GetUrl).ToList(),
                        LastModifiedText = a.LatestMedia != null
                            ? TimeOperations.GetTimeAgo(a.LatestMedia.CreatedUtc)
                            : "",
                        LikeUrl = $"/api/Albums/{a.MediaAlbumId}/Like",
                        LikeGroups = LikeOperations.MakeLikeGroups(a.Likes),
                        CommentUrl = $"/api/Albums/{a.MediaAlbumId}/Comment",
                        Comments = CommentOperations.GetComments(a.CommentThread)
                    })
                .ToList();
        }

        public static async Task<AlbumDetail> GetAlbumAsync(
            ApplicationDbContext dbContext,
            int albumId)
        {
            MediaAlbum entity = await dbContext.MediaAlbums
                .Include(a => a.UserMedias.Select(um => um.Description))
                .Include(a => a.UserMedias.Select(um => um.Likes.Select(like => like.User)))
                .Include(a => a.UserMedias.Select(um => um.CommentThread.Comments.Select(c => c.Text)))
                .Include(a => a.UserMedias.Select(um => um.CommentThread.Comments.Select(c => c.User.Avatar)))
                .Include(a => a.Likes.Select(like => like.User))
                .Include(a => a.CommentThread.Comments.Select(c => c.Text))
                .Include(a => a.CommentThread.Comments.Select(c => c.User.Avatar))
                 .SingleOrDefaultAsync(a => a.MediaAlbumId == albumId);
            if (entity == null)
            {
                return null;
            }

            return new AlbumDetail
            {
                Title = entity.Title,
                UserId = entity.UserId,
                Description = entity.Description,
                Items = entity.UserMedias
                    .Select(
                        um =>
                        new AlbumItem
                        {
                            Id = um.UserMediaId,
                            UserId = um.UserId,
                            Title = um.Title,
                            Description = um.Description?.Content,
                            CreatedTime = um.CreatedUtc.ToString("s"),
                            CreatedTimeAgo = TimeOperations.GetTimeAgo(um.CreatedUtc),
                            MediaUrl = UserMediaOperations.GetUrl(um),
                            LikeUrl = UserMediaOperations.GetLikeUrl(um),
                            LikeGroups = LikeOperations.MakeLikeGroups(um.Likes),
                            CommentUrl = UserMediaOperations.GetCommentUrl(um),
                            Comments = CommentOperations.GetComments(um.CommentThread)
                        })
                    .ToList(),
                LikeUrl = $"/api/Albums/{entity.MediaAlbumId}/Like",
                LikeGroups = LikeOperations.MakeLikeGroups(entity.Likes),
                CommentUrl = $"/api/Albums/{entity.MediaAlbumId}/Comment",
                Comments = CommentOperations.GetComments(entity.CommentThread)
            };
        }

        public static async Task<AlbumSummary> CreateAlbumAsync(
            ApplicationDbContext dbContext,
            UserInfo user,
            AlbumDefinition createRequest)
        {
            var entity = new MediaAlbum
            {
                User = user,
                Title = createRequest.Title,
                Description = createRequest.Description
            };
            dbContext.MediaAlbums.Add(entity);
            await dbContext.SaveChangesAsync();
            await SearchOperations.IndexAlbumAsync(entity);

            return new AlbumSummary
            {
                Id = entity.MediaAlbumId,
                UserId = user.UserInfoId,
                Title = entity.Title,
                Description = entity.Description,
                SampleMediaUrls = new string[0]
            };
        }

        public static async Task<HttpStatusCode> ChangeAlbumAsync(
            ApplicationDbContext dbContext,
            UserInfo user,
            int albumId,
            AlbumDefinition modifyRequest)
        {
            MediaAlbum entity = await dbContext.MediaAlbums.SingleOrDefaultAsync(
                a => a.MediaAlbumId == albumId);
            if (entity == null)
            {
                return HttpStatusCode.NotFound;
            }

            if (entity.UserId != user.UserInfoId)
            {
                return HttpStatusCode.Forbidden;
            }

            entity.Title = modifyRequest.Title;
            entity.Description = modifyRequest.Description;

            await dbContext.SaveChangesAsync();
            await SearchOperations.IndexAlbumAsync(entity);

            return HttpStatusCode.OK;
        }

        public static async Task AddMediaToAlbumAsync(
            ApplicationDbContext dbContext,
            MediaAlbum albumEntity,
            UserMedia mediaEntity,
            AddImageToAlbum createRequest)
        {
            albumEntity.UserMedias.Add(mediaEntity);
            mediaEntity.MediaAlbum = albumEntity;
            if (mediaEntity.State == UserMediaState.UploadedButUnused)
            {
                mediaEntity.State = UserMediaState.InUse;
            }

            mediaEntity.Title = createRequest.Title;
            if (!string.IsNullOrWhiteSpace(createRequest.Description))
            {
                mediaEntity.Description = await TextOperations.CreateTextAsync(
                    dbContext, createRequest.Description);
            }

            await dbContext.SaveChangesAsync();
            await SearchOperations.IndexMediaAsync(new[] { mediaEntity });

            if (mediaEntity.Description != null)
            {
                await UserOperations.NotifyMentionsAsync(
                    dbContext, "Album Entry", mediaEntity.UserId, mediaEntity.Description);
            }
        }

        public static async Task<HttpStatusCode> AddOrRemoveAlbumLikeAsync(
            ApplicationDbContext dbContext,
            string userId,
            int entryId,
            LikeRequest like)
        {
            MediaAlbum albumEntity = await dbContext.MediaAlbums
                .SingleOrDefaultAsync(te => te.MediaAlbumId == entryId);
            if (albumEntity == null)
            {
                // The entry id is part of the URL, so return a 404.
                return HttpStatusCode.NotFound;
            }

            return await LikeOperations.AddOrRemoveLikeAsync(
                dbContext,
                userId,
                entryId,
                le => le.MediaAlbumId,
                like);
        }

        public static async Task<HttpStatusCode> AddAlbumCommentAsync(
            ApplicationDbContext dbContext,
            string userId,
            int albumId,
            CommentRequest comment)
        {
            MediaAlbum albumEntity = await dbContext.MediaAlbums
                .SingleOrDefaultAsync(te => te.MediaAlbumId == albumId);
            if (albumEntity == null)
            {
                // The entry id is part of the URL, so return a 404.
                return HttpStatusCode.NotFound;
            }

            return await CommentOperations.AddCommentAsync(
                dbContext,
                userId,
                new CommentItemIds { AlbumId = albumId },
                e => e.CommentThread,
                albumEntity,
                comment);
        }
    }
}
