

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
        public string TotalTatima { get; set; }
        public string PendingTatima { get; set; }
        public string CompletedTatima { get; set; }
        public string PendingAtSOI { get; set; }
        public string PendingAtDepartment { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }

        public List<DashboardViewModel> Districts { get; set; } = new List<DashboardViewModel>();

    }

}
