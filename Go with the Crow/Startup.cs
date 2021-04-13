using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Go_with_the_Crow.Startup))]
namespace Go_with_the_Crow
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
