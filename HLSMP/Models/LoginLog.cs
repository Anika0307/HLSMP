using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HLSMP.Models
{
    public class LoginLog
    {
        public int Id { get; set; }
        
        public int RoleId { get; set; }

        public int DistrictId { get; set; }

        [StringLength(20)]
        public string UserName { get; set; }

        public DateTime LoginTime { get; set; }
       
        public string IPAddress { get; set; }
    }
}
