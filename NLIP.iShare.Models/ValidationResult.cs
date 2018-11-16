namespace NLIP.iShare.Models
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
        public static ValidationResult Valid() => new ValidationResult(true);
        public static ValidationResult Invalid(string error) => new ValidationResult(false, error);
    }
}
