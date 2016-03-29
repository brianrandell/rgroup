using Rg.ApiTypes;
using System.Collections.Generic;

namespace Rg.Web.ViewModels
{
    public class HomeViewModel : ViewModelWithTextEditingBase
    {
        public IList<TimelineEntryDetails> TimelineEntries { get; set; }
    }
}
