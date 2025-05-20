using HLSMP.Data;
using HLSMP.Models;
using HLSMP.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace HLSMP.Controllers
{
    public class GISLabController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GISLabController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var districts = _context.DisMas
            .Select(d => new DisMa
            {
                DisCode = d.DisCode,
                DisName = d.DisName
            }).ToList();

            var viewModel = new GISViewModel
            {
                Districts = districts
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Index(GISViewModel model)
        {
            model.Districts = _context.DisMas
           .Select(d => new DisMa
           {
               DisCode = d.DisCode,
               DisName = d.DisName
           }).ToList();

            ViewBag.Message = $"You selected district code: {model.SelectedDisCode}";
            return View(model);
        }
    }
}
