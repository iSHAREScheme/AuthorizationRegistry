using Microsoft.Extensions.Logging;
using iSHARE.Abstractions;
using iSHARE.Abstractions.Email;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Net;
using System.Threading.Tasks;
using EmailAddress = iSHARE.Abstractions.Email.EmailAddress;
using SendGridEmailAddress = SendGrid.Helpers.Mail.EmailAddress;


namespace iSHARE.EmailClient
{
    public class EmailClient : IEmailClient
    {
        private readonly EmailOptions _emailOptions;
        private readonly ILogger<EmailClient> _logger;

        public EmailClient(EmailOptions emailOptions, ILogger<EmailClient> logger)
        {
            _emailOptions = emailOptions;
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
                From = CreateEmailAddress(_emailOptions.From.Address),
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
            {
                throw new ArgumentException($"Invalid email address from: {from}");
            }

            if (!IsValidEmail(to))
            {
                throw new ArgumentException($"Invalid email address to: {to}");
            }

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
                From = CreateEmailAddress(_emailOptions.From.Address, _emailOptions.From.DisplayName),
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
            => new SendGridClient(_emailOptions.SendGridKey);

        private static SendGridEmailAddress CreateEmailAddress(string address, string name) 
            => new SendGridEmailAddress(address, name);

        private static SendGridEmailAddress CreateEmailAddress(string address) 
            => new SendGridEmailAddress(address);

        private static bool IsValidEmail(string email)
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
