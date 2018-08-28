namespace NLIP.iShare.AuthorizationRegistry.Api.ViewModels
{
    public class SendEmailRequest
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
