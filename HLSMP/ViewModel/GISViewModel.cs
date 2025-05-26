using HLSMP.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HLSMP.ViewModel
{
    public class GISViewModel : IValidatableObject
    {
        public List<SelectListItem> DistrictList { get; set; } = new();
        [ValidateNever]
        public List<SelectListItem> TehsilList { get; set; }
        [ValidateNever]
        public List<SelectListItem> VillageList { get; set; }
        [ValidateNever]
        public List<SelectListItem> VillageStageList { get; set; }

        [Required(ErrorMessage = "District is required.")]
        public string DIS_CODE { get; set; }

        [Required(ErrorMessage = "Tehsil is required.")]
        public string Tehsil { get; set; }

        [Required(ErrorMessage = "Village is required.")]
        public string Village { get; set; }

        [Required(ErrorMessage = "Village Stage is required.")]
        public string VillageStage { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Work Date is required.")]
        public DateTime? WorkDate { get; set; }

        [Required(ErrorMessage = "Upload a file.")]
        public IFormFile UploadedFile { get; set; }

        [Required(ErrorMessage = "Total Tatima is required.")]
        public int? TotalTatima { get; set; }

        [Required(ErrorMessage = "Completed Tatima is required.")]
        public int? Completed { get; set; }

        [Required(ErrorMessage = "Pending Tatima is required.")]
        public int? Pending { get; set; }

        [Required(ErrorMessage = "Please specify whether work is done.")]
        public bool? IsWorkDone { get; set; } = true;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if ((Completed ?? 0) + (Pending ?? 0) > (TotalTatima ?? 0))
            {
                yield return new ValidationResult("Completed + Pending must be less than Total Tatima", new[] { "TotalTatima" });
            }
        }
    }
}


