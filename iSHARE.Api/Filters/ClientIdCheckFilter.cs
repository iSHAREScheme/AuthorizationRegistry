using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace iSHARE.Api.Filters
{
    public class ClientIdCheckFilter : ActionFilterAttribute
    {
        private readonly ILogger _logger;
        private readonly List<string> _safeList;

        public ClientIdCheckFilter
            (ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _logger = loggerFactory.CreateLogger("ClientIdCheckFilter");
            _safeList = CreateSafeList(configuration["AdminSafeList"] ?? "", configuration["BotIp"]);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var remoteIp = context.HttpContext.Connection.RemoteIpAddress;
            _logger.LogInformation($"Executing request from Remote IpAddress: {remoteIp}");

            if (!remoteIp.IsAllowed(_safeList))
            {
                _logger.LogInformation(
                    $"Forbidden Request from Remote IP address: {remoteIp}");
                context.Result = new StatusCodeResult(401);
                return;
            }

            base.OnActionExecuting(context);
        }

        private static List<string> CreateSafeList(string adminSafeList, string botIp)
        {
            var ips = adminSafeList.Split(';').Where(c => !string.IsNullOrEmpty(c)).ToList();

            if (!string.IsNullOrEmpty(botIp))
            {
                ips.Add(botIp);
            }

            return ips;
        }
    }
}
