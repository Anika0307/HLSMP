using System.ComponentModel.DataAnnotations;

namespace HLSMP.Models
{
    public class LoginDetail
    {
        public int Id { get; set; }

        [Required]
        public string District { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
