using Rg.ServiceCore.Operations;
using Rg.ApiTypes;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Rg.Api.Controllers
{
    /// <summary>
    /// Provides access to information about media (e.g. images).
    /// </summary>
    public class MediaController : ApiControllerBase
    {
        /// <summary>
        /// Gets information about a particular media item.
        /// </summary>
        /// <param name="id">The id of the item of interest.</param>
        /// <returns>Information about the item.</returns>
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
