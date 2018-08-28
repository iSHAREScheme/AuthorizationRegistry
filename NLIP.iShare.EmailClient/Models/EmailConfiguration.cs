using NLIP.iShare.Abstractions.Email;
using System;
using NLIP.iShare.Abstractions;

namespace NLIP.iShare.EmailClient.Models
{
    public class EmailConfiguration
    {
        public string SendGridKey { get; }
        public EmailAddress From { get; }

        public EmailConfiguration(string sendGridKey, EmailAddress from)
        {
            sendGridKey.NotNullOrEmpty(nameof(sendGridKey));
            From = from;
            SendGridKey = sendGridKey;
            
        }

    }
}
