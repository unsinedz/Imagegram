using System.ComponentModel.DataAnnotations;
using Imagegram.Api.Validation;
using Microsoft.AspNetCore.Http;

namespace Imagegram.Api.Models.Api
{
    public class PostInput
    {
        [Required]
        [LimitPostImageSize]
        public IFormFile Image { get; set; }
    }
}