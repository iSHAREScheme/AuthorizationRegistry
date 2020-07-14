using System;
using System.IO;
using iSHARE.Api.Interfaces;
using iSHARE.Api.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace iSHARE.Api.Configurations
{
    public static class FileProvider
    {
        public static IServiceCollection AddFileProvider(this IServiceCollection services, IWebHostEnvironment environment)
        {
            services.AddSingleton<Func<string, IFileInfo>>(srv => filename =>
            {
                IFileProvider fileProvider = new PhysicalFileProvider(Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "Resources",
                     environment.EnvironmentName ?? "Development"));

                var file = fileProvider.GetFileInfo(filename);
                if (!file.Exists)
                {
                    throw new FileNotFoundException("File not found");
                }

                return file;
            });
            services.AddSingleton<ICapabilitiesService, CapabilitiesService>();

            return services;
        }
    }
}
