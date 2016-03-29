using Rg.ApiTypes;

namespace Rg.Web.ViewModels
{
    public class AlbumViewModel : ViewModelWithTextEditingBase
    {
        public int AlbumId { get; set; }
        public string PosessiveInformalName { get; set; }
        public string FullName { get; set; }

        public AlbumDetail Detail { get; set; }
    }
}
