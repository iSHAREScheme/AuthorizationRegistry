using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using NLIP.iShare.Api;

namespace NLIP.iShare.AuthorizationRegistry
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
            => WebHost.CreateDefaultBuilder(args)
               .UseDefaultWebHostOptions<Startup>()
               .Build();
    }
}
