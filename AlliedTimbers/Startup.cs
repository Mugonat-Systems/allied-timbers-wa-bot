using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(AlliedTimbers.Startup))]



namespace AlliedTimbers
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}