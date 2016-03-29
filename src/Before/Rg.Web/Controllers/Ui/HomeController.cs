using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Rg.ServiceCore.Operations;
using Rg.ApiTypes;
using Rg.Web.ViewModels;

namespace Rg.Web.Controllers.Ui
{
    [Authorize]
    public class HomeController : UiControllerBase
    {
        public async Task<ActionResult> Index()
        {
            IList<TimelineEntryDetails> results = await TimelineOperations.GetTimelineItemsAsync(
DbContext, null);
            return View(await SetVmEditorInfo(new HomeViewModel
            {
                TimelineEntries = results
            }));
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
    }
}