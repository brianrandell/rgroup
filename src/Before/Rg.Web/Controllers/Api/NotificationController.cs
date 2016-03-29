using Rg.ServiceCore.DbModel;
using Rg.ServiceCore.Operations;
using Rg.ApiTypes;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Rg.Web.Controllers.Api
{
    public class NotificationController : ApiControllerBase
    {
        [Route("api/Notification/Register")]
        public async Task Post(NotificationRegistration registration)
        {
            UserInfo user = await GetUserInfoAsync();
            if (!await NotificationOperations.RegisterAsync(user, registration))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
        }
    }
}
