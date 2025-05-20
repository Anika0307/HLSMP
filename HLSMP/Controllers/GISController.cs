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
        //var userJson = HttpContext.Session.GetString("LoginUser");
        //LoginDetail user = null;

        //    if (!string.IsNullOrEmpty(userJson))
        //    {
        //        user = JsonSerializer.Deserialize<LoginDetail>(userJson);
        //    }

        public IActionResult GISView()
        {
            var userJson = HttpContext.Session.GetString("LoginUser");
            LoginDetail user = null;

            if (!string.IsNullOrEmpty(userJson))
            {
                user = JsonSerializer.Deserialize<LoginDetail>(userJson);
            }

            var viewModel = new DisTehVillViewModel
            {
                Districts = _context.DisMas
                    .Where(d => d.DisCode == Convert.ToString(user.DistrictId))
                    .Select(d => new SelectListItem
                    {
                        Text = d.DisName,
                        Value = d.DisCode
                    }).ToList(),

                //Reasons = _context.TblReasonMas
                //    .Select(r => new SelectListItem
                //    {
                //        Text = r.Reason,
                //        Value = r.ReasonId.ToString()
                //    }).ToList()
            };

            return View(viewModel);
        }




        [HttpPost]
        public IActionResult Search(DisTehVillViewModel model)
        {
            // Do something with model.SelectedDistrictId, SelectedTehsilId, SelectedVillageId
            return Content($"You selected: District {model.SelectedDistrictId}, Tehsil {model.SelectedTehsilId}, Village {model.SelectedVillageId}");
        }

        [HttpGet]
        public JsonResult OnGetVillages(string tehsilCode)
        {
            var Villages = _context.VilMas
                 .Where(v => v.TehCode == Convert.ToInt32(tehsilCode))
                 .Select(v => new SelectListItem
                 {
                     Text = v.VilName,
                     Value = v.VilCode
                 }).ToList();

            return new JsonResult(Villages);
        }

        [HttpGet]
        public JsonResult OnGetTehsil(string districtId)
        {
            var Tehsils = _context.TehMasLgdUpdateds
                //.Where(v => v.DisCode == Convert.ToInt32(districtId))
                .Select(v => new SelectListItem
                {
                    Text = v.TehName,
                    Value = v.TehCode.ToString()
                }).ToList();

            return new JsonResult(Tehsils);
        }
    }
}
