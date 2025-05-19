using Microsoft.AspNetCore.Mvc.Rendering;

namespace HLSMP.ViewModel
{
    public class DisTehVillViewModel
    {
      
            public int SelectedDistrictId { get; set; }
            public int SelectedTehsilId { get; set; }
            public int SelectedVillageId { get; set; }

            public List<SelectListItem> Districts { get; set; }
            public List<SelectListItem> Tehsils { get; set; }
            public List<SelectListItem> Villages { get; set; }
        
    }
}
