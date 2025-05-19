using Microsoft.EntityFrameworkCore;

namespace HLSMP.ViewModel
{
    public class TatimaSummary
    {

        public string District { get; set; }
        public int TotalTatimaDistrict { get; set; }
        public int TatimasPending { get; set; }
        public int TatimasCompleted { get; set; }
        public int PendingAtSOI { get; set; }
        public int PendingAtDepartment { get; set; }
    }
}
