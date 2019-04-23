using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace iSHARE.Api.Filters
{
    public class ClientIdCheckFilter : ActionFilterAttribute
    {
        private readonly ILogger _logger;
        private readonly string _safeList;

        public ClientIdCheckFilter
            (ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _logger = loggerFactory.CreateLogger("ClientIdCheckFilter");
            _safeList = configuration["AdminSafeList"] ?? "";
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation(
                $"Remote IpAddress: {context.HttpContext.Connection.RemoteIpAddress}");

            var remoteIp = context.HttpContext.Connection.RemoteIpAddress;
            _logger.LogDebug($"Request from Remote IP address: {remoteIp}");

            var ip = _safeList.Split(';').Where(c => !string.IsNullOrEmpty(c)).ToList();
            if (ip.Any())
            {
                var bytes = remoteIp.GetAddressBytes();
                var badIp = true;
                foreach (var address in ip)
                {
                    var testIp = IPAddress.Parse(address);
                    if (testIp.GetAddressBytes().SequenceEqual(bytes))
                    {
                        badIp = false;
                        break;
                    }
                }

                if (badIp)
                {
                    _logger.LogInformation(
                        $"Forbidden Request from Remote IP address: {remoteIp}");
                    context.Result = new StatusCodeResult(401);
                    return;
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
