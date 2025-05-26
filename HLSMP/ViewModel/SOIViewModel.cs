using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;


namespace HLSMP.ViewModel
{
    public class SOIViewModel
    {
        public string DIS_CODE { get; set; }
        public string TEH_CODE { get; set; }

        [Required(ErrorMessage = "Remarks are required.")]
        public string Remarks { get; set; }
        public List<SelectListItem> DistrictList { get; set; } = new();
        public List<SelectListItem> TehsilList { get; set; } = new();
        public List<SOIVillages> Villages { get; set; }
    }
}
