
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
        private DashboardViewModel GetTehsilWiseData(string districtCode)
        {
            DashboardViewModel model = new DashboardViewModel();
            try
            {
                using SqlConnection conn = new(_configuration.GetConnectionString("DefaultConnection"));
                using SqlCommand cmd = new(@"sp_GetDashboardDataTehWise", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    model.DistrictName = reader["DIS_NAME"]?.ToString();
                    model.DistrictCode = reader["DIS_CODE"]?.ToString();
                    model.TotalTatima = Convert.ToInt32(reader["TotalTatima"]);
                    model.PendingTatima = Convert.ToInt32(reader["PendingTatima"]);
                    model.CompletedTatima = Convert.ToInt32(reader["CompletedTatima"]);
                    model.PendingAtSOI = Convert.ToInt32(reader["PendingAtSOI"]);
                    model.PendingAtDepartment = Convert.ToInt32(reader["PendingAtDepartment"]);

                }
                return model;
            }
            catch (Exception ex)
            {
                return model;
            }
        }
    }
}
