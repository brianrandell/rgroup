using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Rg.Web.Startup))]
namespace Rg.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
