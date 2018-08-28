namespace NLIP.iShare.AuthorizationRegistry.Core
{
    public class ValidationResult
    {
        private ValidationResult(bool success, string error = null)
        {
            Success = success;
            Error = error;
        }

        public bool Success { get; }
        public string Error { get; }

        internal static ValidationResult Valid()
        {
            return new ValidationResult(true);
        }

        internal static ValidationResult Invalid(string error)
        {
            return new ValidationResult(false, error);
        }
    }
}
