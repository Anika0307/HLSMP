using System.ComponentModel.DataAnnotations;

namespace HLSMP.Models
{
    public class StatusModel
    {
        [Key]

        public int TatimaID { get; set; }

        public int StatusId { get; set; }
        [StringLength(50)]

        public string Status { get; set; }
    }
}
