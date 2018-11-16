using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using NLIP.iShare.Api;

namespace NLIP.iShare.AuthorizationRegistry.Api
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
