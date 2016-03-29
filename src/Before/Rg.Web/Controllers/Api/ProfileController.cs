using Rg.ServiceCore.DbModel;
using Rg.ServiceCore.Operations;
using Rg.ApiTypes;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Rg.Web.Controllers.Api
{
    [Authorize]
    public class ProfileController : ApiControllerBase
    {
        [Route("api/Profile")]
        public async Task<UserSettings> GetAsync()
        {
            UserInfo user = await GetUserInfoAsync();
            return UserOperations.GetUserSettings(user);
        }

        [Route("api/Profile/SetAvatarImage")]
        public async Task PostAvatar(SetImage request)
        {
            UserInfo user = await GetUserInfoAsync();
            HttpStatusCode result = await UserOperations.SetAvatarImage(
                DbContext, user, request);
            result.ThrowHttpResponseExceptionIfNotSuccessful();
        }

        [Route("api/Profile/SetBannerImage")]
        public async Task PostBanner(SetImage request)
        {
            UserInfo user = await GetUserInfoAsync();
            HttpStatusCode result = await UserOperations.SetBannerImage(
                DbContext, user, request);
            result.ThrowHttpResponseExceptionIfNotSuccessful();
        }

        [Route("api/Profile/EmailWhenMentioned")]
        public async Task PostMentionEmailNotifications([FromBody]bool enable)
        {
            UserInfo user = await GetUserInfoAsync();
            if (enable)
            {
                user.MentionNotificationSettings |= NotifyOptions.Email;
            }
            else
            {
                user.MentionNotificationSettings &= ~NotifyOptions.Email;
            }
            await DbContext.SaveChangesAsync();
        }

        [Route("api/Profile/NotifyWhenMentioned")]
        public async Task PostMentionPushNotifications([FromBody]bool enable)
        {
            UserInfo user = await GetUserInfoAsync();
            if (enable)
            {
                user.MentionNotificationSettings |= NotifyOptions.PushNotification;
            }
            else
            {
                user.MentionNotificationSettings &= ~NotifyOptions.PushNotification;
            }
            await DbContext.SaveChangesAsync();
        }
    }
}
