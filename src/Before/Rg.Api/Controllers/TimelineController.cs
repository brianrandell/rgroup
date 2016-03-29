using Rg.ServiceCore.Operations;
using Rg.ApiTypes;
using Rg.ServiceCore.DbModel;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Rg.Api.Controllers
{
    /// <summary>
    /// Provides access to the timeline.
    /// </summary>
    public class TimelineController : ApiControllerBase
    {
        /// <summary>
        /// Gets recent items on the timeline.
        /// </summary>
        /// <param name="since">
        /// Optional cutoff time in 2015-10-04T19:12:24 format - if present, only
        /// messages newer than this will be returned.
        /// </param>
        /// <returns>A list of timeline entries.</returns>
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

        /// <summary>
        /// Gets the details for a particular timeline item.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The timeline entry.</returns>
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

        /// <summary>
        /// Adds a new entry to the timeline.
        /// </summary>
        /// <param name="createMessage">The entry to add.</param>
        /// <returns></returns>
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
            UserInfo user = await GetUserInfoAsync();
            HttpStatusCode result = await TimelineOperations.AddOrRemoveTimelineEntryLikeAsync(
                DbContext, user.UserInfoId, entryId, likeRequest);
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
            UserInfo user = await GetUserInfoAsync();
            HttpStatusCode result = await TimelineOperations.AddTimelineEntryCommentAsync(
                DbContext, user.UserInfoId, entryId, commentRequest);
            result.ThrowHttpResponseExceptionIfNotSuccessful();
        }
    }
}
