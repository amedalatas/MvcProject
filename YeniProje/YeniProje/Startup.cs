using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(YeniProje.Startup))]
namespace YeniProje
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
