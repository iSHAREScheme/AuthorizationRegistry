using Flurl.Http;
using Flurl.Http.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;

namespace NLIP.iShare.TokenClient
{
    public static class Configuration
    {
        public static void AddTokenClient(this IServiceCollection services, IReadOnlyCollection<TokenSource> wellKnownTokenSources)
        {
            services.AddTransient<ITokenClient, TokenClient>();
            services.AddTransient<IAssertionService, AssertionService>();

            foreach (var source in wellKnownTokenSources)
            {
                FlurlHttp.ConfigureClient(source.BaseUri.ToString(), settings =>
                {
                    settings.Settings.HttpClientFactory = new BypassCertificateValidation(source.Thumbprint);
                    settings.AllowAnyHttpStatus();
                });
            }
        }

        public static void AddTokenClient(this IServiceCollection services) 
            => services.AddTokenClient(new TokenSource[] { });

        public static void AddTokenClient(this IServiceCollection services, TokenSource tokenSource)
            => services.AddTokenClient(new[] { tokenSource });

        internal class BypassCertificateValidation : DefaultHttpClientFactory
        {
            private readonly string _sourceThumbprint;

            public BypassCertificateValidation(string sourceThumbprint)
            {
                _sourceThumbprint = sourceThumbprint;
            }

            public override HttpMessageHandler CreateMessageHandler()
            {
                var handler = new SocketsHttpHandler();

                if (!string.IsNullOrEmpty(_sourceThumbprint))
                {
                    handler.SslOptions.RemoteCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors)
                        => string.Equals(cert.GetCertHashString(HashAlgorithmName.SHA1),
                            _sourceThumbprint, StringComparison.OrdinalIgnoreCase);
                }

                return handler;
            }
        }
    }
}
