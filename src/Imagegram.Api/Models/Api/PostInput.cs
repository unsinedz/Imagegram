using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Imagegram.Api.Models.Api
{
    public class PostInput
    {
        [Required]
        public IFormFile Image { get; set; }
    }
}