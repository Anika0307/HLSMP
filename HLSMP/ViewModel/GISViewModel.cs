using HLSMP.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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
        [ValidateNever]
        public List<SelectListItem> TehsilList { get; set; }
        [ValidateNever]
        public List<SelectListItem> VillageList { get; set; }
        [ValidateNever]
        public List<SelectListItem> VillageStageList { get; set; }

        public bool? IsWorkDone { get; set; }

        [DataType(DataType.Date)]
        public DateTime? WorkDate { get; set; }

        [Required]
        [Display(Name = "Upload Document")]
        public IFormFile UploadedFile { get; set; }

        [Required]
        public int TotalTatima { get; set; }

        [Required]
        public int Completed { get; set; }

        [Required]
        public int Pending { get; set; }

    }
}
