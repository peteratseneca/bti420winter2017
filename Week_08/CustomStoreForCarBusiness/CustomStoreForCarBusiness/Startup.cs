using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CustomStoreForCarBusiness.Startup))]
namespace CustomStoreForCarBusiness
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
