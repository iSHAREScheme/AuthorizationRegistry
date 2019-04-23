using System;

namespace iSHARE.TokenClient.Models
{
    public class ClientAssertion
    {
        public ClientAssertion(string subject, string audience)
        {
            Subject = subject;
            Issuer = subject;
            Audience = audience;
            JwtId = Guid.NewGuid().ToString("N");
            IssuedAt = DateTime.UtcNow;
            Expiration = IssuedAt.AddSeconds(30);
        }

        public ClientAssertion()
        {
        }

        public string Subject { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string JwtId { get; set; }
        public DateTime IssuedAt { get; set; }
        public DateTime Expiration { get; set; }
    }
}
