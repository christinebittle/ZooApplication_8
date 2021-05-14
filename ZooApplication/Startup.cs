using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ZooApplication.Startup))]
namespace ZooApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
