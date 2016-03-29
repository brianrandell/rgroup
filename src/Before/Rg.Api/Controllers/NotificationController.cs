using Rg.ServiceCore.DbModel;
using Rg.ServiceCore.Operations;
using Rg.ApiTypes;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Rg.Api.Controllers
{
    /// <summary>
    /// Push notification registration handling.
    /// </summary>
    public class NotificationController : ApiControllerBase
    {
        /// <summary>
        /// Registers for push notifications.
        /// </summary>
        /// <param name="registration">
        /// Information about the registration.
        /// </param>
        /// <returns></returns>
        [Route("api/Notification/Register")]
        public async Task Post(NotificationRegistration registration)
        {
            Trace.TraceInformation("Notification Register POST");
            UserInfo user = await GetUserInfoAsync();
            Trace.TraceInformation($"Register found user {user.UserInfoId}");
            if (!await NotificationOperations.RegisterAsync(user, registration))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
        }
    }
}
