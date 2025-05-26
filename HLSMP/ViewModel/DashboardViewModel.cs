

namespace HLSMP.ViewModel
{
    public class DashboardViewModel
    {

        public string DistrictName { get; set; }
        public string TehsilName { get; set; }
        public string VillageName { get; set; }
        public string DistrictCode { get; set; }
        public string TehsilCode { get; set; }
        public string VillageCode { get; set; }
        public int TotalTatima { get; set; }
        public int PendingTatima { get; set; }
        public int CompletedTatima { get; set; }
        public int PendingAtSOI { get; set; }
        public int PendingAtDepartment { get; set; }

        public List<DashboardViewModel> Districts { get; set; } = new List<DashboardViewModel>();

    }

  



}
