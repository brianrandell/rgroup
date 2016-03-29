using Rg.ApiTypes;

namespace Rg.Web.ViewModels
{
    public class TimelineItemViewModel : ViewModelWithTextEditingBase
    {
        public int ItemId { get; set; }
        public TimelineEntryDetails Item { get; set; }
    }
}
