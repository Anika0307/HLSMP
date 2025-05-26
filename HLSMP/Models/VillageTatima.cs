using System.ComponentModel.DataAnnotations;

namespace HLSMP.Models
{
    public class VillageTatima
    {
        [StringLength(2)]
        public string Dist_Code { get; set; }
        [StringLength(3)]
        public string Teh_Code { get; set; }

        [Key]
        [StringLength(5)]
        public string VillageCode { get; set; }

        public int TotalTatima { get; set; }

        public int Completed { get; set; }
        public int Pending { get; set; }

        public int StatusCode { get; set; }

        [StringLength(100)]
        public string? UploadedDocument { get; set; }

        [StringLength(1)]
        public string? IsWorkDone { get; set; }
        public string? Remarks { get; set; }

        public DateTime? WorkDate { get; set; }
        public int VillageStageCode { get; set; }
        [StringLength(100)]
        public string IPAddress { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        [StringLength(50)]
        public string? UpdatedBy { get; set; }
        public string CreatedBy { get; set; }


    }
}