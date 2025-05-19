using HLSMP.Data;
using HLSMP.Models;
using HLSMP.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace HLSMP.Controllers
{
    public class GISController : Controller
    {
       
        private readonly ApplicationDbContext _context;

        public GISController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult GISView()
        {
            var userJson = HttpContext.Session.GetString("LoginUser");
            LoginDetail user = null;
            if (!string.IsNullOrEmpty(userJson))
            {
                user = JsonSerializer.Deserialize<LoginDetail>(userJson);
            }

            var districts = _context.DisMas
        .Select(d => new SelectListItem
        {
            Value = d.DisCode.ToString(),
            Text = d.DisName
        }).ToList();

            var model = new DisTehVillViewModel
            {
                Districts = districts,
                SelectedDistrictId = user?.DistrictId ?? 0,
                Tehsils = new List<SelectListItem>(),
                Villages = new List<SelectListItem>()
            };

            return View(model);
            
        }

        [HttpPost]
        public IActionResult Search(DisTehVillViewModel model)
        {
            // Do something with model.SelectedDistrictId, SelectedTehsilId, SelectedVillageId
            return Content($"You selected: District {model.SelectedDistrictId}, Tehsil {model.SelectedTehsilId}, Village {model.SelectedVillageId}");
        }

        [HttpGet]
        public async Task<JsonResult> GetTehsils(string districtId)
        {
            var tehsils = await _context.TehMasLgdUpdateds
                .Where(t => t.DisCode == Convert.ToInt32(districtId))
                .Select(t => new { id = t.TehCode, name = t.TehName })
                .ToListAsync();

            return Json(tehsils);
        }

        [HttpGet]
        public async Task<JsonResult> GetVillages(string tehsilId)
        {
            var villages = await _context.VilMas
                .Where(v => v.TehCode == Convert.ToInt32(tehsilId))
                .Select(v => new { id = v.VilCode, name = v.VilName })
                .ToListAsync();

            return Json(villages);
        }
    }
}
