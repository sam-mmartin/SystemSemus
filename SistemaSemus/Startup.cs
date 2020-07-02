using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SistemaSemus.Startup))]
namespace SistemaSemus
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}