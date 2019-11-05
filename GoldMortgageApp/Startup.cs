using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GoldMortgageApp.Startup))]
namespace GoldMortgageApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
