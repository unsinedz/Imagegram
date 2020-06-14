using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Imagegram.Api.Validation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class LimitPostImageSizeAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "File size cannot exceed {0} Kb.";

        public LimitPostImageSizeAttribute()
        {
            ErrorMessage = DefaultErrorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            if (!(value is IFormFile formFile))
                throw new InvalidOperationException($"{nameof(LimitPostImageSizeAttribute)} can only be applied to {nameof(IFormFile)} properties.");

            var maxFileSizeKb = ((IOptions<PostOptions>)validationContext.GetService(typeof(IOptions<PostOptions>))).Value.MaxFileSizeKb;
            if (formFile.Length > maxFileSizeKb * 1e3)
                return new ValidationResult(GetErrorMessage(maxFileSizeKb));

            return ValidationResult.Success;
        }


        private string GetErrorMessage(int maxFileSizeKb) => string.Format(ErrorMessageString, maxFileSizeKb);
    }
}