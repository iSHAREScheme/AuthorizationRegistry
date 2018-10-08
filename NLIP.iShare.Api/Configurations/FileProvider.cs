using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.IO;

namespace NLIP.iShare.Api.Configurations
{
    public static class FileProvider
    {
        public static IServiceCollection AddFileProvider(this IServiceCollection services)
        {
            services.AddSingleton<Func<string, IFileInfo>>(srv => filename =>
            {
                IFileProvider fileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Resources"));

                var file = fileProvider.GetFileInfo(filename);
                if (!file.Exists)
                {
                    throw new FileNotFoundException("File not found");
                }

                return file;
            });
            return services;
        }
    }
}
