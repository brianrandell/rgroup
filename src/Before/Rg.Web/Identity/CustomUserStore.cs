using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Rg.ServiceCore.DbModel;

namespace Rg.Web.Identity
{
    public class CustomUserStore :
        IUserStore<ApplicationUser>,
        IUserLoginStore<ApplicationUser>,
        IUserEmailStore<ApplicationUser>,
        IUserLockoutStore<ApplicationUser, string>,
        IUserTwoFactorStore<ApplicationUser, string>
    {
        private readonly ApplicationDbContext _db;
        public CustomUserStore(ApplicationDbContext db)
        {
            _db = db;
        }

        public Task AddLoginAsync(ApplicationUser user, UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        public async Task CreateAsync(ApplicationUser user)
        {
            UserInfo entity = user.ToDbEntity();
            _db.UserInfos.Add(entity);
            await _db.SaveChangesAsync();
        }

        public Task DeleteAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }

        public async Task<ApplicationUser> FindAsync(UserLoginInfo login)
        {
            ApplicationUser user = null;
            if (login.LoginProvider == "Microsoft")
            {
                UserInfo entity = await _db.UserInfos.SingleOrDefaultAsync(
                    u => u.MicrosoftAccountProviderKeyWeb == login.ProviderKey);
                if (entity != null)
                {
                    user = new ApplicationUser(entity);
                }

            }
            return user;
        }

        public async Task<ApplicationUser> FindByEmailAsync(string email)
        {
            UserInfo entity = await _db.UserInfos.SingleOrDefaultAsync(
                u => u.Email == email);
            return entity == null ? null : new ApplicationUser(entity);
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId)
        {
            UserInfo entity = await _db.UserInfos.SingleOrDefaultAsync(
                u => u.UserInfoId == userId);
            return entity == null ? null : new ApplicationUser(entity);
        }

        public Task<ApplicationUser> FindByNameAsync(string userName)
        {
            // We don't have a unique username concept (because login is only
            // supported through external ids), but the SignInManager and
            // UserManager manager force us to implement IUser<string>, which
            // requires a UserName. Ours just returns the same as the Id.
            return FindByIdAsync(userName);
        }

        public Task<string> GetEmailAsync(ApplicationUser user)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(ApplicationUser user)
        {
            // Email implicitly confirmed by virtue of MSA-based login.
            return Task.FromResult(true);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user)
        {
            return Task.FromResult<IList<UserLoginInfo>>(new[]
            {
                new UserLoginInfo("Microsoft", user.MicrosoftAccountProviderKey)
            });
        }

        public Task RemoveLoginAsync(ApplicationUser user, UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailAsync(ApplicationUser user, string email)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        //Task IUserStore<ApplicationUser, string>.CreateAsync(ApplicationUser user)
        //{
        //    throw new NotImplementedException();
        //}

        Task IUserStore<ApplicationUser, string>.DeleteAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        //Task<ApplicationUser> IUserStore<ApplicationUser, string>.FindByIdAsync(string userId)
        //{
        //    throw new NotImplementedException();
        //}

        //Task<ApplicationUser> IUserStore<ApplicationUser, string>.FindByNameAsync(string userName)
        //{
        //    throw new NotImplementedException();
        //}

        // SignInManager.ExternalSignInAsync requires IUserLockoutStore, and
        // IUserTwoFactorStore<ApplicationUser, string>, unfortunately.
        // Seems weird because if the user attempts and fails an external login, we never
        // see it, so there's really nothing to track.
        Task<int> IUserLockoutStore<ApplicationUser, string>.GetAccessFailedCountAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        Task<bool> IUserLockoutStore<ApplicationUser, string>.GetLockoutEnabledAsync(ApplicationUser user)
        {
            return Task.FromResult(false);
        }

        Task<DateTimeOffset> IUserLockoutStore<ApplicationUser, string>.GetLockoutEndDateAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        Task<int> IUserLockoutStore<ApplicationUser, string>.IncrementAccessFailedCountAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        Task IUserLockoutStore<ApplicationUser, string>.ResetAccessFailedCountAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        Task IUserLockoutStore<ApplicationUser, string>.SetLockoutEnabledAsync(ApplicationUser user, bool enabled)
        {
            return Task.FromResult(default(object));
        }

        Task IUserLockoutStore<ApplicationUser, string>.SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset lockoutEnd)
        {
            throw new NotImplementedException();
        }

        Task<bool> IUserTwoFactorStore<ApplicationUser, string>.GetTwoFactorEnabledAsync(ApplicationUser user)
        {
            return Task.FromResult(false);
        }
        Task IUserTwoFactorStore<ApplicationUser, string>.SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled)
        {
            throw new NotImplementedException();
        }
    }
}
