using Microsoft.AspNetCore.Mvc.Rendering;


namespace HLSMP.ViewModel
{
    public class SOIViewModel
    {
        public string DIS_CODE { get; set; }
        public string TEH_CODE { get; set; }


        public List<SelectListItem> DistrictList { get; set; } = new();
        public List<SelectListItem> TehsilList { get; set; } = new();
        public List<SOIVillages> Villages { get; set; }
    }
}
