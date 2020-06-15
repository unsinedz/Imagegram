using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Imagegram.Api.Validation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RestrictPostImageExtensionsAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "Only following extensions are allowed: {0}.";

        public RestrictPostImageExtensionsAttribute()
        {
            ErrorMessage = DefaultErrorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            if (!(value is IFormFile formFile))
                throw new InvalidOperationException($"{nameof(RestrictPostImageExtensionsAttribute)} can only be applied to {nameof(IFormFile)} properties.");

            var postOptions = (IOptions<PostOptions>)validationContext.GetService(typeof(IOptions<PostOptions>));
            var supportedImageFormats = postOptions.Value.SupportedImageFormats
                .Split(new char[] { ' ', ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (!supportedImageFormats.Contains(Path.GetExtension(formFile.FileName), StringComparer.OrdinalIgnoreCase))
                return new ValidationResult(GetErrorMessage(string.Join(", ", supportedImageFormats)));

            return ValidationResult.Success;
        }


        private string GetErrorMessage(string supportedExtensions) => string.Format(ErrorMessageString, supportedExtensions);
    }
}