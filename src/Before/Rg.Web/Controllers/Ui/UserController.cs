using Rg.ServiceCore.DbModel;
using Rg.Web.ViewModels;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Rg.Web.Controllers.Ui
{
    [Authorize]
    public class UserController : UiControllerBase
    {
        [Route("User/{userId}")]
        public async Task<ActionResult> Page(string userId)
        {
            UserInfo item = await GetUserInfoAsync(userId);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(await SetVmEditorInfo(new UserViewModel
            {
            }));
        }
    }
}
