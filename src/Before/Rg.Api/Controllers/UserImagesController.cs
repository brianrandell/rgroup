using System.Data.Entity;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Rg.ServiceCore.DbModel;
using Rg.ApiTypes;
using Rg.ServiceCore.Operations;

namespace Rg.Api.Controllers
{
    /// <summary>
    /// Provides access to images.
    /// </summary>
    public class UserImagesController : ApiControllerBase
    {
        /// <summary>
        /// Gets a user-uploaded image.
        /// </summary>
        /// <param name="id">The image id.</param>
        /// <param name="extension">The image extension. (Present so that image URLs look
        /// like image URLs, e.g. api/userimages/123.jpg)</param>
        /// <returns>The image (as a byte stream, with the content type indicated in
        /// the headers).</returns>
        [Route("api/userimages/{id}.{extension}")]
        public async Task<HttpResponseMessage> Get(int id, string extension)
        {
            if (string.IsNullOrEmpty(extension))
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            UserMedia entity = await DbContext.UserMedias
                .Include(um => um.Data)
                .SingleOrDefaultAsync(um => um.UserMediaId == id);
            if (entity == null || entity.Extension != extension)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            var content = new ByteArrayContent(entity.Data.ImageData);
            content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping("x." + extension));
            return new HttpResponseMessage
            {
                Content = content
            };
        }

        /// <summary>
        /// Likes (or hugs or unlikes, etc.) a particular image.
        /// </summary>
        /// <param name="id">Id of the image.</param>
        /// <param name="extension">Extension (jpg, png, etc).</param>
        /// <param name="likeRequest">Whether to like, unlike, hug etc.</param>
        /// <returns></returns>
        [Route("api/userimages/{id}.{extension}/Like")]
        public async Task PostLike(int id, string extension, LikeRequest likeRequest)
        {
            UserInfo user = await GetUserInfoAsync();
            HttpStatusCode result = await UserMediaOperations.AddOrRemoveMediaLikeAsync(
                DbContext, user.UserInfoId, id, likeRequest);
            result.ThrowHttpResponseExceptionIfNotSuccessful();
        }

        /// <summary>
        /// Adds a comment to an album.
        /// </summary>
        /// <param name="mediaId">
        /// The item to comment on.
        /// </param>
        /// <param name="extension">
        /// File extension. (Media items are identified as, e.g. 1.jpg, 2.png, etc.)
        /// </param>
        /// <param name="commentRequest">
        /// Details of the comment to add.
        /// </param>
        /// <returns></returns>
        [Route("api/userimages/{mediaId}.{extension}/Comment")]
        public async Task PostComment(int mediaId, string extension, CommentRequest commentRequest)
        {
            UserInfo user = await GetUserInfoAsync();
            HttpStatusCode result = await UserMediaOperations.AddMediaCommentAsync(
                DbContext, user.UserInfoId, mediaId, commentRequest);
            result.ThrowHttpResponseExceptionIfNotSuccessful();
        }
    }
}
