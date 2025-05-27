
using HLSMP.CustomAttribute;
using HLSMP.Data;
using HLSMP.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data;

namespace HLSMP.Controllers
{

    public class DashboardController : Controller
    {
        private readonly IConfiguration _configuration;

        public DashboardController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            DashboardViewModel model = GetDistrictWiseData();
            return View(model);
        }

        [HttpGet]
        private DashboardViewModel GetDistrictWiseData()
        {
            DashboardViewModel model = new DashboardViewModel();
            try
            {
                using SqlConnection conn = new(_configuration.GetConnectionString("DefaultConnection"));
                using SqlCommand cmd = new(@"sp_GetDashboardDataDistWise", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var district = new DashboardViewModel
                    {
                        DistrictName = reader["DIS_NAME"]?.ToString(),
                        DistrictCode = reader["DIS_CODE"]?.ToString(),
                        TotalTatima = Convert.ToInt32(reader["TotalTatima"]),
                        PendingTatima = Convert.ToInt32(reader["PendingTatima"]),
                        CompletedTatima = Convert.ToInt32(reader["CompletedTatima"]),
                        PendingAtSOI = Convert.ToInt32(reader["PendingAtSOI"]),
                        PendingAtDepartment = Convert.ToInt32(reader["PendingAtDepartment"])
                    };

                    model.Districts.Add(district);
                }
            }
            catch (Exception ex)
            {
                // Optional: Log error
            }

            return model;
        }

        //------------------Tehsil Wise Data------------------//

        [HttpGet]
        public JsonResult GetTehsilWiseData(string districtCode)
        {
            List<DashboardViewModel> tehsils = new();
            try
            {
                using SqlConnection conn = new(_configuration.GetConnectionString("DefaultConnection"));
                using SqlCommand cmd = new("sp_GetDashboardDataTehWise", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Dist_Code", districtCode);

                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    tehsils.Add(new DashboardViewModel
                    {
                        TehsilName = reader["TEH_NAME"]?.ToString(),
                        TehsilCode = reader["Teh_Code"]?.ToString(),
                        DistrictCode = reader["DIS_CODE"]?.ToString(),
                        DistrictName = reader["DIS_NAME"]?.ToString(),
                        TotalTatima = Convert.ToInt32(reader["TotalTatima"]),
                        PendingTatima = Convert.ToInt32(reader["Pending"]),
                        CompletedTatima = Convert.ToInt32(reader["Completed"]),
                        PendingAtSOI = Convert.ToInt32(reader["PendingAtSOI"]),
                        PendingAtDepartment = Convert.ToInt32(reader["PendingAtDepartment"])
                    });
                }
            }
            catch (Exception)
            {
                // Optionally log error
            }

            return Json(tehsils);
        }
        //------------------Village Wise Data------------------//
        [HttpGet]
        public JsonResult GetVillageWiseData(string tehsilCode, string districtCode)
        {
            List<DashboardViewModel> villages = new();
            try
            {
                using SqlConnection conn = new(_configuration.GetConnectionString("DefaultConnection"));
                using SqlCommand cmd = new("sp_HLSMPGetDashboardDataVlgWise", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@Dist_Code", districtCode);
                cmd.Parameters.AddWithValue("@Teh_Code", tehsilCode);

                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    villages.Add(new DashboardViewModel
                    {
                        DistrictName = reader["DIS_NAME"]?.ToString(),
                        DistrictCode = reader["DIS_CODE"]?.ToString(),
                        TehsilName = reader["TEH_NAME"]?.ToString(),
                        TehsilCode = reader["TEH_CODE"]?.ToString(),
                        VillageName = reader["VillageName"]?.ToString(), // <-- check actual name
                        VillageCode = reader["VillageCode"]?.ToString(), // <-- check actual name
                        TotalTatima = reader["TotalTatima"] != DBNull.Value ? Convert.ToInt32(reader["TotalTatima"]) : 0,
                        PendingTatima = reader["Pending"] != DBNull.Value ? Convert.ToInt32(reader["Pending"]) : 0,
                        CompletedTatima = reader["Completed"] != DBNull.Value ? Convert.ToInt32(reader["Completed"]) : 0,
                        PendingAtSOI = reader["PendingAtSOI"] != DBNull.Value ? Convert.ToInt32(reader["PendingAtSOI"]) : 0,
                        PendingAtDepartment = reader["PendingAtDepartment"] != DBNull.Value ? Convert.ToInt32(reader["PendingAtDepartment"]) : 0
                    });
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("Error in GetVillageWiseData: " + ex.Message);
                return Json(new { success = false, message = "Error occurred" });
            }

            return Json(villages);
        }

    }
}
