using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Rg.ServiceCore.DbModel;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Linq;

namespace Rg.Web.Identity
{
    // We implement IUser<T> because SignInManager requires it, and SignInManager
    // handles external login providers.
    public class ApplicationUser : IUser<string>
    {
        /// <summary>
        /// Creates a user object representing a user already in our database.
        /// </summary>
        /// <param name="entity"></param>
        public ApplicationUser(UserInfo entity)
        {
            Id = entity.UserInfoId;
            MicrosoftAccountProviderKey = entity.MicrosoftAccountProviderKeyWeb;
            Email = entity.Email;
            Name = entity.Name;
        }

        /// <summary>
        /// Creates a user object representing a new user not yet in our database.
        /// </summary>
        /// <param name="info"></param>
        public ApplicationUser(ExternalLoginInfo info)
        {
            if (info.Login.LoginProvider != "Microsoft")
            {
                throw new ArgumentException($"Unsupport login provider '${info.Login.LoginProvider}'. Must be 'Microsoft'");
            }

            Id = Guid.NewGuid().ToString("d");
            MicrosoftAccountProviderKey = info.Login.ProviderKey;
            Email = info.Email;
            Name = info.ExternalIdentity.Name;
        }

        public string Id { get; }
        public string MicrosoftAccountProviderKey { get; }
        public string Email { get; }
        public string Name { get; }

        public UserInfo ToDbEntity()
        {
            return new UserInfo
            {
                UserInfoId = Id,
                MicrosoftAccountProviderKeyWeb = MicrosoftAccountProviderKey,
                Email = Email,
                Name = Name
            };
        }

        // ASP.NET identity has the somewhat annoying requirement that all users have both
        // a unique id, and also a unique username. The username makes sense if that's how
        // users are logging in, but if you're using just external ids, it gets in the way.
        // So we return our internal id as the 'username'.
        string IUser<string>.UserName
        {
            get
            {
                return Id;
            }
            set
            {
                // Since we don't use UserName as ASP.NET identity intends, this is a code
                // path we should never hit.
                throw new NotSupportedException();
            }
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            // The UserManager sets the Name from our UserName. However, we don't like its
            // 'UserName must be globally unique id' rule, and so our UserName actually
            // returns our Id. We therefore remove the supplied claim and replace it with
            // one containing the displayable user name.
            Claim userNameClaim = userIdentity.Claims.Single(c => c.Type == userIdentity.NameClaimType);
            userIdentity.RemoveClaim(userNameClaim);
            userIdentity.AddClaim(new Claim(userIdentity.NameClaimType, Name));

            return userIdentity;
        }
    }
}