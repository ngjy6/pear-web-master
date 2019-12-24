using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NTU_FYP_REBUILD_17.Startup))]
namespace NTU_FYP_REBUILD_17
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
