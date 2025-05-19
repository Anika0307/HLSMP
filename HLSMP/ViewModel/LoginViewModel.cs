using System.ComponentModel.DataAnnotations;

namespace HLSMP.ViewModel
{
    public class LoginViewModel
    {
        [Required]
        public string Role { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string CaptchaCode { get; set; }

        public string GeneratedCaptcha { get; set; }
    }
}
