using System.ComponentModel.DataAnnotations;

namespace HLSMP.Models
{
    public class VillageTatima
    {
        
        public int Dist_Code { get; set; }

        public int Teh_Code { get; set; }

        [Key]
        public int VillageCode { get; set; }

        public int TotalTatima { get; set; }

        public int Completed {  get; set; }
        public int Pending { get; set; }

        public int StatusCode { get; set; }

        [StringLength(100)]
        public string?  UploadedDocument { get; set; }

    }
}