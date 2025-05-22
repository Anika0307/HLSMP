using System.ComponentModel.DataAnnotations;

namespace HLSMP.ViewModel
{
    public class SOIVillages
    {

        public string VillageName { get; set; }
        public int TotalTatima { get; set; }
        public int CompletedTatima { get; set; }
        public int PendingTatima    { get; set; }
        public string? IsWorkDone { get; set; }
        [Display(Name = "Upload Document")]
        public string? UploadedFile { get; set; }

        public DateTime? WorkDate { get; set; }

        public string? VillageStage { get; set; }


        public string VillageCode { get; set; }
    }
}
