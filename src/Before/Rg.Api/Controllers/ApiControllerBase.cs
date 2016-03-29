using Rg.ServiceCore.DbModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Security.Claims;
using Microsoft.Azure.AppService.ApiApps.Service;
using System.Data.Entity;
using System.Collections.Generic;
using System.Configuration;
using System;
using System.Net.Http;
using System.Security.Principal;
using Microsoft.Azure.Mobile.Server.Authentication;
using Newtonsoft.Json;

namespace Rg.Api.Controllers
{
    public class ApiControllerBase : ApiController
    {
        private ApplicationDbContext _dbContext;
        private Task<Dictionary<string, string>> _apiTokenDictionaryTask;
        private Task<UserInfo> _userInfoTask;
        private static HashSet<string> _masterUserEmails;

        protected ApplicationDbContext DbContext
        {
            get
            {
                return LazyInitializer.EnsureInitialized(
                    ref _dbContext,
                    ApplicationDbContext.Create);
            }
        }

        protected Task<Dictionary<string, string>> GetApiTokenClaimsAsync()
        {
            return LazyInitializer.EnsureInitialized(
                ref _apiTokenDictionaryTask,
                async () =>
                {
                    var msaCreds = await User.GetAppServiceIdentityAsync<MicrosoftAccountCredentials>(Request);
                    if (msaCreds != null)
                    {
                        return msaCreds.Claims.ToDictionary(c => c.Key, c => c.Value); 
                    }

                    // For requests coming in via Swagger, the MSA token seems not to be available. However,
                    // in that case the set of claims available in the User object seems to be the same as would
                    // have been in the MSA token, so we return those instead.
                    var claimsUser = User as ClaimsPrincipal;
                    if (claimsUser != null)
                    {
                        return claimsUser.Claims.ToDictionary(c => c.Type, c => c.Value);
                    }

                    return new Dictionary<string, string>();
                });
        }

        protected async Task<string> GetEmailFromMsaAsync()
        {
            var msaCreds = await User.GetAppServiceIdentityAsync<MicrosoftAccountCredentials>(Request);

            if (msaCreds != null)
            {
                string result;
                using (var client = new HttpClient())
                {
                    result = await client.GetStringAsync("https://apis.live.net/v5.0/me?access_token=" + msaCreds.AccessToken);
                    var body = JsonConvert.DeserializeObject<MsaMeResponse>(result);
                    return body.emails.account;
                }
            }
            return null;
        }

        protected static HashSet<string> MasterEmails
        {
            get
            {
                return LazyInitializer.EnsureInitialized(
                    ref _masterUserEmails,
                    () => new HashSet<string>(ConfigurationManager.AppSettings["MasterUsers"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)));
            }
        }

        protected Task<UserInfo> GetUserInfoAsync(string userId = null)
        {
            if (userId != null)
            {
                return DbContext.UserInfos.SingleOrDefaultAsync(u => u.UserInfoId == userId);
            }

            return LazyInitializer.EnsureInitialized(
                ref _userInfoTask,
                async () =>
                {
                    string id = (await GetApiTokenClaimsAsync())[ClaimTypes.NameIdentifier];

                    UserInfo info = await DbContext.UserInfos.SingleOrDefaultAsync(u => u.MicrosoftAccountProviderKeyApi == id);
                    return info;
                });
        }

        public class Emails
        {
            public string preferred { get; set; }
            public string account { get; set; }
            public object personal { get; set; }
            public object business { get; set; }
        }

        public class MsaMeResponse
        {
            public string id { get; set; }
            public string name { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string link { get; set; }
            public object gender { get; set; }
            public Emails emails { get; set; }
            public string locale { get; set; }
            public string updated_time { get; set; }
        }
    }
}
