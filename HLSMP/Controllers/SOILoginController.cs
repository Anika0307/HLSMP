using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using HLSMP.ViewModel;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;
using HLSMP.Models;

namespace HLSMP.Controllers
{
    public class SOILoginController : Controller
    {
        private readonly IConfiguration _configuration;

        public SOILoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var model = new SOIViewModel
            {
                DistrictList = GetDistricts()
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(SOIViewModel model)
        {
            model.DistrictList = GetDistricts();
            model.TehsilList = GetTehsils(model.DIS_CODE);

            if (!string.IsNullOrEmpty(model.DIS_CODE) && !string.IsNullOrEmpty(model.TEH_CODE))
            {
                model.Villages = GetVillages(model.DIS_CODE, model.TEH_CODE);
            }

            return View(model);
        }


        [HttpPost]
        public JsonResult GetTehsilsByDistrict(string DIS_CODE)
        {
            var tehsils = GetTehsils(DIS_CODE);
            return Json(tehsils);
        }


        private List<SelectListItem> GetDistricts()
        {
            var districts = new List<SelectListItem>();
            using SqlConnection conn = new(_configuration.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new("select DIS_CODE, DIS_NAME as District from DIS_MAS", conn);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                districts.Add(new SelectListItem
                {
                    Value = reader["DIS_CODE"].ToString(),
                    Text = reader["District"].ToString()
                });
            }
            return districts;
        }



        private List<SelectListItem> GetTehsils(string DIS_CODE)
        {
            var tehsils = new List<SelectListItem>();
            using SqlConnection conn = new(_configuration.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new("SELECT TEH_NAME as Tehsil,TEH_CODE FROM TEH_MAS_LGD_UPDATED WHERE DIS_CODE =@DIS_CODE", conn);
            cmd.Parameters.AddWithValue("@DIS_CODE", DIS_CODE);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
                while (reader.Read())
                {
                    tehsils.Add(new SelectListItem
                    {
                        Value = reader["TEH_CODE"].ToString(), // This will be the posted value
                        Text = reader["Tehsil"].ToString()   // This will be the visible text
                    });
                }
            return tehsils;
        }

        private List<SOIVillages> GetVillages(string DIS_CODE, string TEH_CODE)
        {
            var villages = new List<SOIVillages>();
            using SqlConnection conn = new(_configuration.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new(@"  SELECT v.VIL_NAME as [Village Name],vt.TotalTatima as [Total Tatimas], vt.Pending as [Pending Tatimas],vt.Completed as [Completed Tatimas]
                                 FROM VillageTatimas as vt inner join VIL_MAS as v on vt.VillageCode= v.VIL_CODE 
                                 WHERE Dist_Code = @DIS_CODE 
                                 AND vt.Teh_Code = @TEH_CODE 
                                 AND StatusCode = 2", conn);
            cmd.Parameters.AddWithValue("@DIS_CODE", DIS_CODE);
            cmd.Parameters.AddWithValue("@TEH_CODE", TEH_CODE);
            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                villages.Add(new SOIVillages
                {
                    VillageName = Convert.ToString(reader["Village Name"]),
                    TotalTatima = Convert.ToInt32(reader["Total Tatimas"]),
                    CompletedTatima = Convert.ToInt32(reader["Pending Tatimas"]),
                    PendingTatima = Convert.ToInt32(reader["Total Tatimas"])


                });
            }
            return villages;
        }



    }

}
