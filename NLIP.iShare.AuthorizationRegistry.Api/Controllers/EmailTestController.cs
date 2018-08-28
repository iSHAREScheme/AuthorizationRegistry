using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NLIP.iShare.Abstractions.Email;
using NLIP.iShare.AuthorizationRegistry.Api.ViewModels;
using System;
using System.Threading.Tasks;

namespace NLIP.iShare.AuthorizationRegistry.Api.Controllers
{
    [Route("api/emails")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class EmailTestController : Controller
    {
        private readonly IEmailClient _emailClient;
        private readonly ILogger<EmailTestController> _logger;
        public EmailTestController(IEmailClient emailClient, ILogger<EmailTestController> logger)
        {
            _emailClient = emailClient;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]SendEmailRequest email)
        {
            try
            {
                await _emailClient.Send(
                    email.From,
                    email.To,
                    email.Subject,
                    email.Body);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,"An error occured while sending the email");
                return BadRequest();
            }
        }
    }
    
}