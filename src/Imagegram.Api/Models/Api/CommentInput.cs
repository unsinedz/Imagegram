using System.ComponentModel.DataAnnotations;

namespace Imagegram.Api.Models.Api
{
    public class CommentInput
    {
        [Required, MaxLength(500)]
        public string Content { get; set; }
    }
}