using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CQRS.NET.Startup))]
namespace CQRS.NET
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
