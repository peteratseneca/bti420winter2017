using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TourOfTemplateV2CustomStore.Startup))]
namespace TourOfTemplateV2CustomStore
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
