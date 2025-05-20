using HLSMP.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HLSMP.ViewModel
{
    public class GISViewModel
    {
        public string DIS_CODE { get; set; }

        public string Tehsil { get; set; }
        public string Village { get; set; }
        public string VillageStage { get; set; }

        public List<SelectListItem> DistrictList { get; set; } = new();
        public List<SelectListItem> TehsilList { get; set; }
        public List<SelectListItem> VillageList { get; set; }
        public List<SelectListItem> VillageStageList { get; set; }

        public bool? IsWorkDone { get; set; }

        [DataType(DataType.Date)]
        public DateTime? WorkDate { get; set; }

        [Required]
        [Display(Name = "Upload Document")]
        public IFormFile UploadedFile { get; set; }

    }
}
