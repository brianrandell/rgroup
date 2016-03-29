using Rg.ServiceCore.DbModel;
using Rg.ServiceCore.Operations;
using Rg.ApiTypes;
using System.Collections.Generic;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Rg.Web.Controllers.Api
{
    [Authorize]
    public class AlbumsController : ApiControllerBase
    {
        [Route("api/Albums")]
        public async Task<IList<AlbumSummary>> Get()
        {
            UserInfo user = await GetUserInfoAsync();
            return await AlbumOperations.GetAlbumSummariesAsync(
                DbContext, user);
        }

        [Route("api/Users/{userId}/Albums")]
        public async Task<IList<AlbumSummary>> Get(string userId)
        {
            UserInfo user = await GetUserInfoAsync(userId);
            return await AlbumOperations.GetAlbumSummariesAsync(
                DbContext, user);
        }

        [Route("api/Albums/{albumId}")]
        public async Task<AlbumDetail> Get(int albumId)
        {
            var result = await AlbumOperations.GetAlbumAsync(DbContext, albumId);
            if (result == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return result;
        }

        [Route("api/Albums")]
        public async Task<AlbumSummary> Post(
            AlbumDefinition createRequest)
        {
            UserInfo user = await GetUserInfoAsync();
            AlbumSummary result = await AlbumOperations.CreateAlbumAsync(
                DbContext, user, createRequest);
            if (result == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            return result;
        }

        [Route("api/Albums/{id}")]
        public async Task Put(
            int id,
            AlbumDefinition modifyRequest)
        {
            MediaAlbum album = await DbContext.MediaAlbums.SingleOrDefaultAsync(
                a => a.MediaAlbumId == id);
            if (album == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            if (album.UserId != UserId)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }

            UserInfo user = await GetUserInfoAsync();
            HttpStatusCode result = await AlbumOperations.ChangeAlbumAsync(
                DbContext,
                user,
                id,
                modifyRequest);

            if (result != HttpStatusCode.OK)
            {
                throw new HttpResponseException(result);
            }
        }

        [Route("api/Albums/{albumId}/AddImage")]
        public async Task PostImage(int albumId, AddImageToAlbum createRequest)
        {
            MediaAlbum albumEntity = await DbContext.MediaAlbums
                .SingleOrDefaultAsync(a => a.MediaAlbumId == albumId);
            if (albumEntity == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            if (albumEntity.UserId != UserId)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }


            UserMedia mediaEntity = await DbContext.UserMedias
                .SingleOrDefaultAsync(um => um.UserMediaId == createRequest.MediaId);
            if (mediaEntity == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            if (mediaEntity.UserId != UserId)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }

            await AlbumOperations.AddMediaToAlbumAsync(DbContext, albumEntity, mediaEntity, createRequest);
        }

        [Route("api/Albums/{albumId}/Like")]
        public async Task PostLikeAlbum(int albumId, LikeRequest likeRequest)
        {
            HttpStatusCode result = await AlbumOperations.AddOrRemoveAlbumLikeAsync(
                DbContext, UserId, albumId, likeRequest);
            result.ThrowHttpResponseExceptionIfNotSuccessful();
        }

        /// <summary>
        /// Adds a comment to an album.
        /// </summary>
        /// <param name="albumId">
        /// The item to comment on.
        /// </param>
        /// <param name="commentRequest">
        /// Details of the comment to add.
        /// </param>
        /// <returns></returns>
        [Route("api/Albums/{albumId}/Comment")]
        public async Task PostComment(int albumId, CommentRequest commentRequest)
        {
            HttpStatusCode result = await AlbumOperations.AddAlbumCommentAsync(
                DbContext, UserId, albumId, commentRequest);
            result.ThrowHttpResponseExceptionIfNotSuccessful();
        }
    }
}
