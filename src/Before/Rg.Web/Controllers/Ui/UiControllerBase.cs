using Rg.ServiceCore.DbModel;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Rg.ServiceCore.Operations;
using Rg.Web.ViewModels;
using System.Collections.Generic;
using System.Configuration;
using System;

namespace Rg.Web.Controllers.Ui
{
    public abstract class UiControllerBase : Controller
    {
        private ApplicationDbContext _dbContext;
        private string _userId;
        private static HashSet<string> _masterUserEmails;

        protected ApplicationDbContext DbContext
        {
            get
            {
                return LazyInitializer.EnsureInitialized(
                    ref _dbContext,
                    () => Request.GetOwinContext().Get<ApplicationDbContext>());
            }
        }

        protected string UserId
        {
            get
            {
                return LazyInitializer.EnsureInitialized(
                    ref _userId,
                    () =>
                    {
                        Claim claim = (User.Identity as ClaimsIdentity)?.Claims?.SingleOrDefault(
                            c => c.Type == ClaimTypes.NameIdentifier);
                        return claim?.Value;
                    });
            }
        }

        public static HashSet<string> MasterEmails
        {
            get
            {
                return LazyInitializer.EnsureInitialized(
                    ref _masterUserEmails,
                    () => new HashSet<string>(ConfigurationManager.AppSettings["MasterUsers"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)));
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            UserInfo user = DbContext.UserInfos
                .Include(u => u.Banner)
                .SingleOrDefault(u => u.UserInfoId == UserId);
            if (user == null)
            {
                filterContext.Result = new HttpStatusCodeResult(401);
                return;
            }
            ViewBag.BannerUrl = UserOperations.GetBannerUrl(user);

            base.OnActionExecuting(filterContext);
        }

        protected Task<UserInfo> GetUserInfoAsync(string userId = null)
        {
            return DbContext.UserInfos.FindAsync(userId ?? UserId);
        }

        protected async Task<T> SetBasicVmInfo<T>(T viewModel)
            where T : ViewModelBase
        {
            UserInfo user = await DbContext.UserInfos
                .Include(u => u.Avatar)
                .SingleAsync(u => u.UserInfoId == UserId);

            viewModel.IsMasterUser = MasterEmails.Contains(user.Email);
            viewModel.UserId = UserId;

            viewModel.AvatarUrl = UserOperations.GetAvatarUrl(user);

            return viewModel;
        }

        protected async Task<T> SetVmEditorInfo<T>(T viewModel)
            where T : ViewModelWithTextEditingBase
        {
            var allUsersInfo = await DbContext.UserInfos
                .Include(u => u.Avatar)
                .ToListAsync();

            IDictionary<string, ViewModelWithTextEditingBase.User> allUsers = allUsersInfo
                .ToDictionary(
                    u => u.Email,
                    u => new ViewModelWithTextEditingBase.User
                    {
                        Name = u.Name,
                        Email = u.Email,
                        AvatarUrl = UserOperations.GetAvatarUrl(u)
                    });
            viewModel.AllUsers = allUsers;

            return await SetBasicVmInfo(viewModel);
        }
    }
}
