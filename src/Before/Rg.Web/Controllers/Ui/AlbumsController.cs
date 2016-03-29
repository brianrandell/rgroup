using System.Threading.Tasks;
using System.Web.Mvc;
using Rg.ServiceCore.DbModel;
using Rg.ServiceCore.Operations;
using Rg.ApiTypes;
using Rg.Web.ViewModels;

namespace Rg.Web.Controllers.Ui
{
    [Authorize]
    public class AlbumsController : UiControllerBase
    {
        [Route("Albums")]
        public async Task<ActionResult> Index(string userId = null)
        {
            UserInfo albumUser = await GetUserInfoAsync(userId);
            if (albumUser == null)
            {
                return HttpNotFound();
            }

            var vm = new AlbumsViewModel
            {
                UserId = UserId,
                AlbumOwnerId = userId,
                FullName = albumUser.Name,
                PosessiveInformalName = UserOperations.GetInformalPosessiveName(albumUser),
                AvatarUrl = UserOperations.GetAvatarUrl(albumUser),
                Albums = await AlbumOperations.GetAlbumSummariesAsync(DbContext, albumUser)
            };
            return View("Index", vm);
        }

        [Route("Albums/{albumId}")]
        public async Task<ActionResult> Index(int albumId)
        {
            AlbumDetail albumDetail = await AlbumOperations.GetAlbumAsync(DbContext, albumId);
            if (albumDetail == null)
            {
                return HttpNotFound();
            }

            UserInfo albumUser = await GetUserInfoAsync(albumDetail.UserId);
            AlbumViewModel vm = await SetVmEditorInfo(new AlbumViewModel
            {
                AlbumId = albumId,
                FullName = albumUser.Name,
                PosessiveInformalName = UserOperations.GetInformalPosessiveName(albumUser),
                AvatarUrl = UserOperations.GetAvatarUrl(albumUser),
                Detail = albumDetail
            });
            return View(albumDetail.UserId == UserId ? "OwnedAlbum" : "Album", vm);
        }
    }
}