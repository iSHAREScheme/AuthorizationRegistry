using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using iSHARE.Api;

namespace iSHARE.AuthorizationRegistry.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args)
                .Build()
                .Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseDefaultWebHostOptions<Startup>();
    }
}
