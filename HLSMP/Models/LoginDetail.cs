using System.ComponentModel.DataAnnotations;

namespace HLSMP.Models
{
    public class LoginDetail
    {
        public int Id { get; set; }

        [Required]
        public int RoleId { get; set; }

       
        public int DistrictId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }


        [StringLength(20)]
        [Required]
        public string Password { get; set; }

        public bool IsActive { get; set; }

    }
}
