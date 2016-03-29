using Rg.ApiTypes;
using System.Collections.Generic;

namespace Rg.Web.ViewModels
{
    public class AlbumsViewModel : ViewModelWithTextEditingBase
    {
        public string AlbumOwnerId { get; set; }
        public string PosessiveInformalName { get; set; }
        public string FullName { get; set; }

        public IList<AlbumSummary> Albums { get; set; }
    }
}
