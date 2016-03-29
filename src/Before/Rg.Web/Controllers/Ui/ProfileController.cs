using Rg.ServiceCore.DbModel;
using Rg.ServiceCore.Operations;
using Rg.Web.ViewModels;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Rg.Web.Controllers.Ui
{
    [Authorize]
    public class ProfileController : UiControllerBase
    {
        // GET: Profile
        public async Task<ActionResult> Index()
        {
            UserInfo user = await GetUserInfoAsync();
            var vm = new ProfileViewModel
            {
                UserId = UserId,
                Settings = UserOperations.GetUserSettings(user)
            };
            return View(await SetBasicVmInfo(vm));
        }
    }
}