using Rg.ServiceCore.DbModel;
using Rg.ServiceCore.Operations;
using Rg.Web.ViewModels;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Rg.Web.Controllers.Ui
{
    [Authorize]
    public class InvitationsController : UiControllerBase
    {
        // GET: Invitations
        public async Task<ActionResult> Index()
        {
            UserInfo user = await GetUserInfoAsync();
            if (!MasterEmails.Contains(user.Email))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            var current = await InvitationOperations.GetCurrentInvitations(DbContext);

            var vm = new InvitationsViewModel
            {
                OutstandingInvitations = current.Outstanding,
                AcceptedInvitations = current.Accepted
            };
            return View(await SetBasicVmInfo(vm));
        }
    }
}