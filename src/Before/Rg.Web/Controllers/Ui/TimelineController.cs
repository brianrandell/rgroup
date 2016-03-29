using Rg.ServiceCore.Operations;
using Rg.ApiTypes;
using Rg.Web.ViewModels;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Rg.Web.Controllers.Ui
{
    [Authorize]
    public class TimelineController : UiControllerBase
    {
        [Route("timeline/{entryId}")]
        public async Task<ActionResult> Item(int entryId)
        {
            TimelineEntryDetails entry = await TimelineOperations.GetTimelineItemAsync(
                DbContext, entryId);
            if (entry == null)
            {
                return HttpNotFound();
            }

            return View(await SetVmEditorInfo(new TimelineItemViewModel
            {
                ItemId = entryId,
                Item = entry
            }));
        }
    }
}
