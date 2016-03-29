using Rg.ServiceCore.DbModel;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity.Owin;
using System.Security.Claims;
using System.Configuration;
using System.Collections.Generic;
using System;

namespace Rg.Web.Controllers.Api
{
    public class ApiControllerBase : ApiController
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

        protected static HashSet<string> MasterEmails
        {
            get
            {
                return LazyInitializer.EnsureInitialized(
                    ref _masterUserEmails,
                    () => new HashSet<string>(ConfigurationManager.AppSettings["MasterUsers"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)));
            }
        }

        protected Task<UserInfo> GetUserInfoAsync(string userId = null)
        {
            return DbContext.UserInfos.FindAsync(userId ?? UserId);
        }
    }
}
