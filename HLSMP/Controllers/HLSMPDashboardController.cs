
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

    public class HLSMPDashboardController : Controller
    {
        private readonly IConfiguration _configuration;

        public HLSMPDashboardController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult HLSMPDashboardView()
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
                using SqlCommand cmd = new(@"sp_HLSMPGetDashboardDataDistWise", conn)
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
                        TotalTatima = reader["TotalTatima"] != DBNull.Value ?Convert.ToString(reader["TotalTatima"]) : "0",
                        PendingTatima = reader["PendingTatima"] != DBNull.Value ? Convert.ToString(reader["PendingTatima"]) : "0",
                        CompletedTatima = reader["CompletedTatima"] != DBNull.Value ? Convert.ToString( reader["CompletedTatima"]) : "0",
                        PendingAtSOI = reader["PendingAtSOI"] != DBNull.Value ? Convert.ToString( reader["PendingAtSOI"]) : "0",
                        PendingAtDepartment = reader["PendingAtDepartment"] != DBNull.Value ? Convert.ToString(reader["PendingAtDepartment"]) : "0"
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
            ViewData["DisCode"] = Convert.ToString(districtCode);
            List<DashboardViewModel> tehsils = new();
            try
            {
                using SqlConnection conn = new(_configuration.GetConnectionString("DefaultConnection"));
                using SqlCommand cmd = new("sp_HLSMPGetDashboardDataTehWise", conn)
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
                        TotalTatima = reader["TotalTatima"] != DBNull.Value ? Convert.ToString(reader["TotalTatima"]) : "0",
                        PendingTatima = reader["Pending"] != DBNull.Value ? Convert.ToString(reader["Pending"]) : "0",
                        CompletedTatima = reader["Completed"] != DBNull.Value ? Convert.ToString(reader["Completed"]) : "0",
                        PendingAtSOI = reader["PendingAtSOI"] != DBNull.Value ? Convert.ToString(reader["PendingAtSOI"]) : "0",
                        PendingAtDepartment = reader["PendingAtDepartment"] != DBNull.Value ? Convert.ToString(reader["PendingAtDepartment"]) : "0"

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
                        DistrictName = reader["DIS_NAME"] != DBNull.Value ? Convert.ToString(reader["DIS_NAME"]) : "0",
                        DistrictCode = reader["DIS_CODE"] != DBNull.Value ? Convert.ToString(reader["DIS_CODE"]) : "",
                        TehsilName = reader["TEH_NAME"] != DBNull.Value ? Convert.ToString(reader["TEH_NAME"]) : "",
                        TehsilCode = reader["TEH_CODE"] != DBNull.Value ? Convert.ToString(reader["TEH_CODE"]) : "0",
                        VillageName = reader["VillageName"] != DBNull.Value ? Convert.ToString(reader["VillageName"]) : "",
                        VillageCode = reader["VillageCode"] != DBNull.Value ? Convert.ToString(reader["VillageCode"]) : "0",
                        TotalTatima = reader["TotalTatima"] != DBNull.Value ? Convert.ToString(reader["TotalTatima"]) : "0",
                        PendingTatima = reader["Pending"] != DBNull.Value ? Convert.ToString(reader["Pending"]) : "0",
                        CompletedTatima = reader["Completed"] != DBNull.Value ? Convert.ToString(reader["Completed"]) : "0",
                        Status = reader["Status"] != DBNull.Value ? Convert.ToString(reader["Status"]) : "0",
                        Reason = reader["Reason"] != DBNull.Value ? Convert.ToString(reader["Reason"]) : "0"
                      
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetVillageWiseData: " + ex.Message);
                return Json(new { success = false, message = "Error occurred" });
            }

            return Json(new { data = villages });
        }
    }
}
