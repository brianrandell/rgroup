using System.Data.Entity;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Rg.ServiceCore.DbModel;
using Rg.ServiceCore.Operations;
using Rg.ApiTypes;

namespace Rg.Web.Controllers.Api
{
    [Authorize]
    public class UserImagesController : ApiControllerBase
    {
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

        [Route("api/userimages/{id}.{extension}/Like")]
        public async Task PostLike(int id, string extension, LikeRequest likeRequest)
        {
            HttpStatusCode result = await UserMediaOperations.AddOrRemoveMediaLikeAsync(
                DbContext, UserId, id, likeRequest);
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
            HttpStatusCode result = await UserMediaOperations.AddMediaCommentAsync(
                DbContext, UserId, mediaId, commentRequest);
            result.ThrowHttpResponseExceptionIfNotSuccessful();
        }
    }
}
