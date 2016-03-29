using Rg.ServiceCore.Operations;
using Rg.ApiTypes;
using System.Threading.Tasks;
using System.Web.Http;

namespace Rg.Api.Controllers
{
    /// <summary>
    /// Searches for items.
    /// </summary>
    public class SearchController : ApiControllerBase
    {
        /// <summary>
        /// Gets items matching a search term.
        /// </summary>
        /// <param name="term">The search term to match.</param>
        /// <returns>The items matching this term.</returns>
        [Route("api/Search")]
        public Task<SearchResults> Get([FromUri] string term)
        {
            return SearchOperations.SearchAsync(DbContext, term);
        }
    }
}
