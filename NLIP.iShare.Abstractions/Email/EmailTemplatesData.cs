using System.Collections.Generic;

namespace NLIP.iShare.Abstractions.Email
{
    public class EmailTemplatesData
    {
        public string ActivateAccountEmailFileName { get; } = "ActivateAccountEmail.html";
        public string ForgotPasswordEmailFileName { get; } = "ForgotPasswordEmail.html";
        public string ResetPasswordEmailFileName { get; } = "ResetPasswordEmail.html";
        public Dictionary<string, string> EmailData { get; set; }
    }
}
