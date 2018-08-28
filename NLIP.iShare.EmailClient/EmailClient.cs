using Microsoft.Extensions.Logging;
using NLIP.iShare.Abstractions.Email;
using NLIP.iShare.EmailClient.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Net;
using System.Threading.Tasks;
using NLIP.iShare.Abstractions;
using EmailAddress = NLIP.iShare.Abstractions.Email.EmailAddress;

namespace NLIP.iShare.EmailClient
{
    public class EmailClient : IEmailClient
    {
        private readonly EmailConfiguration _emailConfiguration;
        private readonly ILogger<EmailClient> _logger;

        public EmailClient(EmailConfiguration emailConfiguration, ILogger<EmailClient> logger)
        {
            _emailConfiguration = emailConfiguration;
            _logger = logger;
        }

        public async Task Send(string to, string subject, string body)
        {
            to.NotNullOrEmpty(nameof(to));
            subject.NotNullOrEmpty(nameof(subject));
            body.NotNullOrEmpty(nameof(body));

            var client = CreateClient();
            var msg = new SendGridMessage
            {
                From = CreateEmailAddress(_emailConfiguration.From.Address),
                Subject = subject,
                HtmlContent = body,
            };
            msg.AddTo(CreateEmailAddress(to));
            var response = await client.SendEmailAsync(msg).ConfigureAwait(false);
            if (response.StatusCode != HttpStatusCode.Accepted)
            {
                _logger.LogInformation(response.ToString());
            }
        }

        public async Task Send(string from, string to, string subject, string body)
        {
            if (!IsValidEmail(from))
                throw new ArgumentException($"Invalid email address from: {from}");
            if(!IsValidEmail(to))
                throw new ArgumentException($"Invalid email address to: {to}");
            subject.NotNullOrEmpty(nameof(subject));
            body.NotNullOrEmpty(nameof(body));

            var client = CreateClient();
            var msg = new SendGridMessage
            {
                From = CreateEmailAddress(from),
                Subject = subject,
                HtmlContent = body,
            };
            msg.AddTo(CreateEmailAddress(to));
            var response = await client.SendEmailAsync(msg).ConfigureAwait(false);
            if (response.StatusCode != HttpStatusCode.Accepted)
            {
                _logger.LogInformation(response.ToString());
            }
        }

        public async Task Send(EmailAddress to, string subject, string body)
        {
            if (to == null)
            {
                throw new ArgumentNullException(nameof(to));
            }

            subject.NotNullOrEmpty(nameof(subject));
            body.NotNullOrEmpty(nameof(body));

            var client = CreateClient();
            var msg = new SendGridMessage
            {
                From = CreateEmailAddress(_emailConfiguration.From.Address, _emailConfiguration.From.DisplayName),
                Subject = subject,
                HtmlContent = body,
            };
            msg.AddTo(CreateEmailAddress(to.Address, to.DisplayName));
            var response = await client.SendEmailAsync(msg).ConfigureAwait(false);
            if (response.StatusCode != HttpStatusCode.Accepted)
            {
                _logger.LogInformation(response.ToString());
            }
        }

        public async Task Send(EmailAddress from, EmailAddress to, string subject, string body)
        {
            if (from == null)
            {
                throw new ArgumentNullException(nameof(from));
            }

            if (to == null)
            {
                throw new ArgumentNullException(nameof(to));
            }

            subject.NotNullOrEmpty(nameof(subject));
            body.NotNullOrEmpty(nameof(body));

            var client = CreateClient();
            var msg = new SendGridMessage
            {
                From = CreateEmailAddress(from.Address, from.DisplayName),
                Subject = subject,
                HtmlContent = body,
            };
            msg.AddTo(CreateEmailAddress(to.Address, to.DisplayName));
            var response = await client.SendEmailAsync(msg).ConfigureAwait(false);
            if (response.StatusCode != HttpStatusCode.Accepted)
            {
                _logger.LogInformation(response.ToString());
            }
        }

        
        private SendGridClient CreateClient()
        {
            return new SendGridClient(_emailConfiguration.SendGridKey);
        }

        private SendGrid.Helpers.Mail.EmailAddress CreateEmailAddress(string address, string name)
        {
            return new SendGrid.Helpers.Mail.EmailAddress(address, name);
        }

        private SendGrid.Helpers.Mail.EmailAddress CreateEmailAddress(string address)
        {
            return new SendGrid.Helpers.Mail.EmailAddress(address);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var emailAddress = new System.Net.Mail.MailAddress(email);
                return emailAddress.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
