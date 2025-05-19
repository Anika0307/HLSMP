
using HLSMP.Data;
using HLSMP.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HLSMP.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            // Step 1: Fetch EF Core model data (Models.TatimaSummary)
            var data = _context.TatimaSummaries
                .FromSqlRaw("EXEC GetDistrictTatimaSummary")
                .ToList();

            // Step 2: Map Models.TatimaSummary to ViewModel.TatimaSummary
            var vmData = data.Select(d => new HLSMP.ViewModel.TatimaSummary
            {
                District = d.District,
                TotalTatimaDistrict = d.TotalTatimaDistrict,
                TatimasPending = d.TatimasPending,
                TatimasCompleted = d.TatimasCompleted,
                PendingAtSOI = d.PendingAtSOI,
                PendingAtDepartment = d.PendingAtDepartment
            }).ToList();

            // Step 3: Populate the DashboardViewModel
            var model = new DashboardViewModel
            {
                TatimaSummaries = vmData
            };

            return View(model);
        }


    }
}
    
