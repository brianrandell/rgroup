using Rg.ServiceCore.Operations;
using Rg.ApiTypes;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Rg.Web.Controllers.Api
{
    [Authorize]
    public class MediaController : ApiControllerBase
    {
        [Route("api/Media/{id}")]
        public async Task<AlbumItem> Get(int id)
        {
            AlbumItem result = await UserMediaOperations.GetMediaAsync(
                DbContext, id);
            if (result == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return result;
        }
    }
}
