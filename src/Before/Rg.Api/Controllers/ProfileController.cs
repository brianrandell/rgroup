using Rg.ServiceCore.DbModel;
using Rg.ServiceCore.Operations;
using Rg.ApiTypes;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace Rg.Api.Controllers
{
    public class ProfileController : ApiControllerBase
    {
        /// <summary>
        /// Verifies that the authenticated user is allowed to use the app. MUST be
        /// called before attempting anything else.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Because API Apps don't seem to provide any sort of "user just authenticated"
        /// notification, there's no straightforward way either to a) discover that
        /// a new (to us) user has authenticated for the first time, or b) have any
        /// say in whether we actually want a particular user to have access to the
        /// app. The API Gateway seems to assume that if the id provider authenticates
        /// someone, they must be OK.
        /// </para>
        /// <para>
        /// Since we do some account setup the first time we see a particular user,
        /// we require that API users hit this endpoint before doing anything else.
        /// (They only need do to it once for any particular user.) If the user
        /// is not allowed in, we will return a 403.
        /// </para>
        /// </remarks>
        [Route("api/Profile/Validate")]
        public async Task<UserSettings> PostValidate()
        {
            UserInfo user = await GetUserInfoAsync();
            if (user == null)
            {
                Dictionary<string, string> claims = await GetApiTokenClaimsAsync();
                string msaKey = claims[ClaimTypes.NameIdentifier];

                // It's possible that we've seen this user before via the web app, but
                // that this is the first time via the API. When that happens, GetUserInfoAsync
                // produces null because the lookup by NameIdentifier claim fails. (This will
                // be the first time we've actually seen the NameIdentifier, so we've not yet
                // had a chance to store it.)
                string email = await GetEmailFromMsaAsync();
                user = await DbContext.UserInfos.SingleOrDefaultAsync(u => u.Email == email);
                if (user != null)
                {
                    // Yes, we've seen this user via the web, so store the NameIdentifier so that
                    // next time we'll know who they are from the Zumo token. (Currently, the Zumo
                    // token doesn't include the email, only the NameIdentifier.)
                    user.MicrosoftAccountProviderKeyApi = msaKey;
                }
                else
                {

                    if (!await UserOperations.CheckNewUserAllowed(DbContext, email))
                    {
                        throw new HttpResponseException(HttpStatusCode.Forbidden);
                    }

                    // User must either be on the master list, or invited.
                    if (!MasterEmails.Contains(email))
                    {
                        Invitation inv = await DbContext.Invitations.SingleOrDefaultAsync(i => i.Email == email);
                        if (inv == null)
                        {
                            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden)
                            {
                                Content = new StringContent("Not invited")
                            });
                        }
                    }

                    string msaName = claims[ClaimTypes.Name];
                    user = new UserInfo
                    {
                        UserInfoId = Guid.NewGuid().ToString("d"),
                        Email = email,
                        MicrosoftAccountProviderKeyApi = msaKey,
                        Name = msaName,
                    };

                    DbContext.UserInfos.Add(user);
                }

                await DbContext.SaveChangesAsync();
            }

            return UserOperations.GetUserSettings(user);
        }

        /// <summary>
        /// Gets the current user settings.
        /// </summary>
        /// <returns>The user's settings.</returns>
        [Route("api/Profile")]
        public async Task<UserSettings> GetAsync()
        {
            UserInfo user = await GetUserInfoAsync();
            return UserOperations.GetUserSettings(user);
        }

        /// <summary>
        /// Sets the user's avatar image.
        /// </summary>
        /// <param name="request">Describes which image to use.</param>
        /// <returns></returns>
        [Route("api/Profile/SetAvatarImage")]
        public async Task PostAvatar(SetImage request)
        {
            UserInfo user = await GetUserInfoAsync();
            HttpStatusCode result = await UserOperations.SetAvatarImage(
                DbContext, user, request);
            result.ThrowHttpResponseExceptionIfNotSuccessful();
        }

        /// <summary>
        /// Sets the user's banner image.
        /// </summary>
        /// <param name="request">Describes which image to use.</param>
        /// <returns></returns>
        [Route("api/Profile/SetBannerImage")]
        public async Task PostBanner(SetImage request)
        {
            UserInfo user = await GetUserInfoAsync();
            HttpStatusCode result = await UserOperations.SetBannerImage(
                DbContext, user, request);
            result.ThrowHttpResponseExceptionIfNotSuccessful();
        }

        /// <summary>
        /// Enable or disable sending of email notifications when mentioned.
        /// </summary>
        /// <param name="setting">Indicates whether to enable or disable.</param>
        /// <returns></returns>
        [Route("api/Profile/EmailWhenMentioned")]
        public async Task PostMentionEmailNotifications(OptionSettings setting)
        {
            UserInfo user = await GetUserInfoAsync();
            if (setting.IsEnabled)
            {
                user.MentionNotificationSettings |= NotifyOptions.Email;
            }
            else
            {
                user.MentionNotificationSettings &= ~NotifyOptions.Email;
            }
            await DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Enable or disable sending of push notifications when mentioned.
        /// </summary>
        /// <param name="setting">Indicates whether to enable or disable.</param>
        /// <returns></returns>
        [Route("api/Profile/NotifyWhenMentioned")]
        public async Task PostMentionPushNotifications(OptionSettings setting)
        {
            UserInfo user = await GetUserInfoAsync();
            if (setting.IsEnabled)
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
