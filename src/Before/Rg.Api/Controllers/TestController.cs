using Microsoft.Azure.AppService.ApiApps.Service;
using Microsoft.Azure.Mobile.Server.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Http;

namespace Rg.Api.Controllers
{
    /// <summary>
    /// Test endpoint to explore API App behavior.
    /// </summary>
    public class TestController : ApiControllerBase
    {
        /// <summary>
        /// Returns the claims associated with the Microsoft Account token,
        /// or reports errors if any occur.
        /// </summary>
        /// <returns></returns>
        public async Task<IList<string>> Get()
        {
            var results = new List<string>();
            try
            {
                var msaCreds = await User.GetAppServiceIdentityAsync<MicrosoftAccountCredentials>(Request);
                
                if (msaCreds != null)
                {
                    results.AddRange(msaCreds.Claims.Select(c => $"MSA: {c.Key}: {c.Value}"));
                    try
                    {
                        using (var client = new HttpClient())
                        {
                            string msaMeJson = await client.GetStringAsync("https://apis.live.net/v5.0/me?access_token=" + msaCreds.AccessToken);
                            results.Add("MSA (me): " + msaMeJson);
                        }
                    }
                    catch (Exception exinner)
                    {
                        results.Add("MSA (me) exception: " + exinner);
                    }
                }
                else
                {
                    results.Add("No MSA claims");
                }
                var user = User as ClaimsPrincipal;
                if (user == null)
                {
                    results.Add("User is not ClaimsPrincipal");
                }
                results.AddRange(user.Claims.Select(c => $"User: {c.Type}: {c.Value}"));

                try
                {
                    string email = await GetEmailFromMsaAsync();
                    if (email != null)
                    {
                        results.Add("Email: " + email);
                    }
                }
                catch (Exception x)
                {
                    // Will tend to happen if the MSA AccessToken has expired.
                    results.Add("Email exception: " + x);
                }

                var userInfo = await GetUserInfoAsync();
                if (userInfo != null)
                {
                    results.Add("UserInfo.UserInfoId: " + userInfo.UserInfoId);
                    results.Add("UserInfo.Email: " + userInfo.Email);
                    results.Add("UserInfo.Name: " + userInfo.Name);
                }
                var claimsAsSeenByRestOfApp = await GetApiTokenClaimsAsync();
                results.AddRange(claimsAsSeenByRestOfApp.Select(c => $"App: {c.Key} {c.Value}"));

            }
            catch (Exception x)
            {
                results.Add("Exception: " + x);
            }
            return results;
        }
    }
}
