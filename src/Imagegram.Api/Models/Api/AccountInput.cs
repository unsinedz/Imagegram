using System.ComponentModel.DataAnnotations;

namespace Imagegram.Api.Models.Api
{
    public class AccountInput
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
    }
}