using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace Rg.Web.Identity
{
    /// <summary>
    /// Manages signing in through external id provider (Microsoft Account) login.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The ASP-NET-supplied base class also supports signing in through other
    /// means such as username and password, but we're only using MSA in this
    /// app.
    /// </para>
    /// <para>
    /// The only thing we change is the default behaviour for creating a new
    /// ClaimsIdentity once login is complete. The default implementation
    /// defers to <c>UserManager.CreateIdentityAsync</c>, which in turn defers
    /// to its ClaimsIdentityFactory. Instead, we call
    /// <see cref="ApplicationUser.GenerateUserIdentityAsync(Microsoft.AspNet.Identity.UserManager{ApplicationUser})"/>
    /// directly. Now as it happens, that also calls <c>UserManager.CreateIdentityAsync</c>,
    /// but the critical difference is that it provides an easy place to adjust
    /// the set of claims before returning. (Without this, we'd need to supply
    /// a custom ClaimsIdentityFactory to be able to adjust the claims.)
    /// </para>
    /// </remarks>
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(
            ApplicationUserManager userManager,
            IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        /// <summary>
        /// Once login completes successfully, ASP.NET identity invokes this,
        /// asking us to build the claims we'd like baked into the token that
        /// will be passed back to the client.
        /// </summary>
        /// <param name="user">
        /// Identifies the user that just logged in.
        /// </param>
        /// <returns>
        /// A task that produces a <c>ClaimsIdentity</c> that will be turned
        /// into the token (embedded in a cookie for web use, or an oauth2 token
        /// for API use) returned to the client.
        /// </returns>
        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager) UserManager);
        }

        public static ApplicationSignInManager Create(
            IdentityFactoryOptions<ApplicationSignInManager> options,
            IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
