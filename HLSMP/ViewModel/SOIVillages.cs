using System.ComponentModel.DataAnnotations;

namespace HLSMP.ViewModel
{
    public class SOIVillages
    {

        public string VillageName { get; set; }
        public string DistrictName { get; set; }
        public string TehsilName { get; set; }
        public int Dist_Code { get; set; }
        public int Teh_Code { get; set; }
        public int Vill_Code { get; set; }
        public int TotalTatima { get; set; }
        public int CompletedTatima { get; set; }
        public int PendingTatima    { get; set; }
        public string? IsWorkDone { get; set; }
        [Display(Name = "Upload Document")]

        //public IFormFile UploadedFile { get; set; }
        public string UploadedDocument { get; set; }

        public DateTime? WorkDate { get; set; }
        public int VillageStageCode { get; set; }
        public string VillageStage { get; set; }

    }
}
