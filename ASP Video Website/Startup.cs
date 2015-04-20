using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ASP_Video_Website.Startup))]
namespace ASP_Video_Website
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
