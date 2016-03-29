using Rg.ServiceCore.Operations;
using Rg.ApiTypes;
using System.Threading.Tasks;
using System.Web.Http;

namespace Rg.Web.Controllers.Api
{
    [Authorize]
    public class SearchController : ApiControllerBase
    {
        [Route("api/Search")]
        public Task<SearchResults> Get([FromUri] string term)
        {
            return SearchOperations.SearchAsync(DbContext, term);
        }
    }
}
