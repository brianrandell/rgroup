using Rg.ServiceCore.DbModel;
using Rg.ServiceCore.Operations;
using Rg.ApiTypes;
using Rg.Web.ViewModels;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Rg.Web.Controllers.Ui
{
    [Authorize]
    public class MediaController : UiControllerBase
    {
        [Route("Media/{mediaId}")]
        public async Task<ActionResult> Item(int mediaId)
        {
            AlbumItem item = await UserMediaOperations.GetMediaAsync(DbContext, mediaId);
            if (item == null)
            {
                return HttpNotFound();
            }

            UserInfo owningUser = await GetUserInfoAsync(item.UserId);
            DbContext.Entry(owningUser).Reference(u => u.Avatar);

            return View(await SetVmEditorInfo(new MediaViewModel
            {
                FullName = owningUser.Name,
                AvatarUrl = UserOperations.GetAvatarUrl(owningUser),
                Detail = item
            }));
        }
    }
}
