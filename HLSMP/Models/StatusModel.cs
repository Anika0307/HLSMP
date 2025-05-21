using System.ComponentModel.DataAnnotations;

namespace HLSMP.Models
{
    public class StatusModel
    {
        [Key]
        public int TatimaID { get; set; }
        public string Status { get; set; }
    }
}
