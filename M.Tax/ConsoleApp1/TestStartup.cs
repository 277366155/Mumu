using Owin;

namespace ConsoleApp1
{
    public class TestStartup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            appBuilder.Use(async (context, next) => {
                await context.Response.WriteAsync("This is response msg.");
            });
        }
    }
}
