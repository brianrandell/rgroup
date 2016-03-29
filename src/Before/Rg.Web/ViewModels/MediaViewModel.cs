using Rg.ApiTypes;

namespace Rg.Web.ViewModels
{
    public class MediaViewModel : ViewModelWithTextEditingBase
    {
        public string FullName { get; set; }
        public AlbumItem Detail { get; set; }
    }
}
