using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace NLIP.iShare.Api.Validation
{
    public sealed class UrlValidatorAttribute : ValidationAttribute
    {
        public UrlValidatorAttribute()
        {
            ErrorMessage = "The url is not valid.";
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }
            
            var valid = Uri.TryCreate(Convert.ToString(value, CultureInfo.CurrentCulture), UriKind.Absolute, out _);

            if (!valid)
            {
                return new ValidationResult(ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }
}
