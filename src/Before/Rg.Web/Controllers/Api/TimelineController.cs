using Rg.ServiceCore.DbModel;
using Rg.ServiceCore.Operations;
using Rg.ApiTypes;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Rg.Web.Controllers.Api
{
    [Authorize]
    public class TimelineController : ApiControllerBase
    {
        public async Task<IList<TimelineEntryDetails>> Get([FromUri] string since = null)
        {
            IList<TimelineEntryDetails> results = await TimelineOperations.GetTimelineItemsAsync(
                DbContext, since);
            if (results == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            return results;
        }

        public async Task<TimelineEntryDetails> Get(int id)
        {
            TimelineEntryDetails result = await TimelineOperations.GetTimelineItemAsync(
                DbContext, id);
            if (result == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return result;
        }

        public async Task Post(CreateTimelineEntry createMessage)
        {
            UserInfo userEntity = await GetUserInfoAsync();
            HttpStatusCode result = await TimelineOperations.AddTimelineEntryAsync(createMessage, DbContext, userEntity);
            result.ThrowHttpResponseExceptionIfNotSuccessful();
        }

        /// <summary>
        /// Likes or unlikes a timeline entry.
        /// </summary>
        /// <param name="entryId">
        /// The item to set or remove a 'like'.
        /// </param>
        /// <param name="likeRequest">
        /// Specifies the kind of 'like', and whether we're setting or removing it.
        /// </param>
        /// <returns></returns>
        [Route("api/Timeline/{entryId}/Like")]
        public async Task PostLike(int entryId, LikeRequest likeRequest)
        {
            HttpStatusCode result = await TimelineOperations.AddOrRemoveTimelineEntryLikeAsync(
                DbContext, UserId, entryId, likeRequest);
            result.ThrowHttpResponseExceptionIfNotSuccessful();
        }

        /// <summary>
        /// Adds a comment to a timeline entry.
        /// </summary>
        /// <param name="entryId">
        /// The item to comment on.
        /// </param>
        /// <param name="commentRequest">
        /// Details of the comment to add.
        /// </param>
        /// <returns></returns>
        [Route("api/Timeline/{entryId}/Comment")]
        public async Task PostComment(int entryId, CommentRequest commentRequest)
        {
            HttpStatusCode result = await TimelineOperations.AddTimelineEntryCommentAsync(
                DbContext, UserId, entryId, commentRequest);
            result.ThrowHttpResponseExceptionIfNotSuccessful();
        }
    }
}
