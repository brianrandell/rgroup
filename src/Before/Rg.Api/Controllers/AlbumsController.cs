using Rg.ServiceCore.DbModel;
using Rg.ServiceCore.Operations;
using Rg.ApiTypes;
using System.Collections.Generic;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Rg.Api.Controllers
{
    /// <summary>
    /// Provides information about and the abilty to modify albums.
    /// </summary>
    public class AlbumsController : ApiControllerBase
    {
        /// <summary>
        /// Gets a list of the user's albums.
        /// </summary>
        /// <returns>
        /// The album's owned by the authenticated user.
        /// </returns>
        [Route("api/Albums")]
        public async Task<IList<AlbumSummary>> Get()
        {
            UserInfo user = await GetUserInfoAsync();
            return await AlbumOperations.GetAlbumSummariesAsync(
                DbContext, user);
        }

        /// <summary>
        /// Gets the albums owned by the specified user.
        /// </summary>
        /// <param name="userId">
        /// The id of the user for which to return albums.
        /// </param>
        /// <returns>
        /// A list of the specified user's albums.
        /// </returns>
        [Route("api/Users/{userId}/Albums/")]
        public async Task<IList<AlbumSummary>> Get(string userId)
        {
            UserInfo user = await GetUserInfoAsync(userId);
            return await AlbumOperations.GetAlbumSummariesAsync(
                DbContext, user);
        }

        /// <summary>
        /// Gets the full details of a particular album.
        /// </summary>
        /// <param name="albumId">
        /// The album for which to fetch details.
        /// </param>
        /// <returns>
        /// Details for the specified album.
        /// </returns>
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

        /// <summary>
        /// Creates a new album.
        /// </summary>
        /// <param name="createRequest">
        /// Information about the album to create.
        /// </param>
        /// <returns>
        /// Information about the album (including its id).
        /// </returns>
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

        /// <summary>
        /// Modifies an existing album.
        /// </summary>
        /// <param name="id">
        /// The id of the album to modify.
        /// </param>
        /// <param name="modifyRequest">
        /// The information to set for this album.
        /// </param>
        /// <returns></returns>
        [Route("api/Albums/{id}")]
        public async Task Put(
            int id,
            AlbumDefinition modifyRequest)

        {
            UserInfo user = await GetUserInfoAsync();
            MediaAlbum album = await DbContext.MediaAlbums.SingleOrDefaultAsync(
                a => a.MediaAlbumId == id);
            if (album == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            if (album.UserId != user.UserInfoId)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }

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

        /// <summary>
        /// Adds a new image to an album.
        /// </summary>
        /// <param name="albumId">
        /// The album to which to add the image.
        /// </param>
        /// <param name="createRequest">
        /// Information about the image to add.
        /// </param>
        /// <returns></returns>
        [Route("api/Albums/{albumId}/AddImage")]
        public async Task PostImage(int albumId, AddImageToAlbum createRequest)
        {
            MediaAlbum albumEntity = await DbContext.MediaAlbums
                .SingleOrDefaultAsync(a => a.MediaAlbumId == albumId);
            if (albumEntity == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            UserInfo user = await GetUserInfoAsync();
            if (albumEntity.UserId != user.UserInfoId)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }


            UserMedia mediaEntity = await DbContext.UserMedias
                .SingleOrDefaultAsync(um => um.UserMediaId == createRequest.MediaId);
            if (mediaEntity == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            if (mediaEntity.UserId != user.UserInfoId)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }

            await AlbumOperations.AddMediaToAlbumAsync(DbContext, albumEntity, mediaEntity, createRequest);
        }

        /// <summary>
        /// Add or remove a 'like' to an album.
        /// </summary>
        /// <param name="albumId">
        /// The album to like (or unlike or hug etc).
        /// </param>
        /// <param name="likeRequest">
        /// Indicates whether we're liking, unliking, hugging, or whatever.
        /// </param>
        /// <returns></returns>
        [Route("api/Albums/{albumId}/Like")]
        public async Task PostLikeAlbum(int albumId, LikeRequest likeRequest)
        {
            UserInfo user = await GetUserInfoAsync();
            HttpStatusCode result = await AlbumOperations.AddOrRemoveAlbumLikeAsync(
                DbContext, user.UserInfoId, albumId, likeRequest);
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
            UserInfo user = await GetUserInfoAsync();
            HttpStatusCode result = await AlbumOperations.AddAlbumCommentAsync(
                DbContext, user.UserInfoId, albumId, commentRequest);
            result.ThrowHttpResponseExceptionIfNotSuccessful();
        }
    }
}
