using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NLIP.iShare.Api.Configurations
{
    public static class JsonSettings
    {
        public static IMvcCoreBuilder AddJsonSettings(this IMvcCoreBuilder mvcBuilder)
        {
            return mvcBuilder
                .AddJsonFormatters(settings =>
                {
                    settings.NullValueHandling = NullValueHandling.Ignore;
                    settings.Converters.Add(
                        new IsoDateTimeConverter
                        {
                            DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'"
                        });
                });
        }
    }
}
