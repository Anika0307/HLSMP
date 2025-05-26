using HLSMP.Models;
using HLSMP.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Net.Sockets;
using System.Net;
using System.Text.Json;
using HLSMP.CustomAttribute;

namespace HLSMP.Controllers
{
    //[AuthorizeRoles(3)]
    public class RevDepartmentController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public RevDepartmentController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        public IActionResult RevDepartmentView()
        {
            var model = new SOIViewModel
            {
                DistrictList = GetDistricts(),
                TehsilList = new List<SelectListItem>(),
                Villages = new List<SOIVillages>()
            };
            string DIS_CODE = "", TEH_CODE = "";
            model.Villages = GetPendingTatima(DIS_CODE, TEH_CODE);
            return View(model);

        }


        [HttpPost]
        public IActionResult RevDepartmentView(SOIViewModel model)
        {
            model.DistrictList = GetDistricts();
            model.TehsilList = GetTehsils(model.DIS_CODE);

            model.Villages = GetPendingTatima(model.DIS_CODE, model.TEH_CODE);
            return View(model);
        }

        [HttpPost]
        public JsonResult GetTehsilsByDistrict(string DIS_CODE)
        {
            var tehsils = GetTehsils(DIS_CODE);
            return Json(tehsils);
        }

        //=====================Bind methods for District and Village======================//
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


        //===========================Get Pending Tatima list=========================//
        private List<SOIVillages> GetPendingTatima(string DIS_CODE, string TEH_CODE)
        {
            var villages = new List<SOIVillages>();

            using SqlConnection conn = new(_configuration.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new("sp_GetVlgTatimasForDepartment", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            if (!string.IsNullOrEmpty(DIS_CODE) && !string.IsNullOrEmpty(TEH_CODE))
            {
                cmd.Parameters.AddWithValue("@DIS_CODE", DIS_CODE);
                cmd.Parameters.AddWithValue("@TEH_CODE", TEH_CODE);
            }
            else if (!string.IsNullOrEmpty(DIS_CODE) && string.IsNullOrEmpty(TEH_CODE))
            {
                cmd.Parameters.AddWithValue("@DIS_CODE", DIS_CODE);
                cmd.Parameters.AddWithValue("@TEH_CODE", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@DIS_CODE", DBNull.Value);
                cmd.Parameters.AddWithValue("@TEH_CODE", DBNull.Value);
            }
            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                villages.Add(new SOIVillages
                {
                    DistrictName = reader["DisName"]?.ToString(),
                    TehsilName = reader["TehName"]?.ToString(),
                    VillageName = reader["VillageName"]?.ToString(),
                    TotalTatima = reader["TotalTatima"] != DBNull.Value ? Convert.ToInt32(reader["TotalTatima"]) : 0,
                    CompletedTatima = reader["Completed"] != DBNull.Value ? Convert.ToInt32(reader["Completed"]) : 0,
                    PendingTatima = reader["Pending"] != DBNull.Value ? Convert.ToInt32(reader["Pending"]) : 0,
                    IsWorkDone = reader["IsWorkDone"]?.ToString() == "Y" ? "Yes" : "No",
                    WorkDate = reader["WorkDate"] != DBNull.Value ? Convert.ToDateTime(reader["WorkDate"]) : (DateTime?)null,
                    VillageStage = reader["VillageStage"]?.ToString(),
                    Dist_Code = Convert.ToString(reader["Dist_Code"]),
                    Teh_Code = Convert.ToString(reader["Teh_Code"]),
                    Vill_Code = Convert.ToString(reader["VillageCode"]),
                    UploadedDocument = Convert.ToString(reader["UploadedDocument"])


                });
            }

            return villages;
        }

        //====================== UpdateTatimaStatus=======================//
        [HttpPost]
        public IActionResult UpdateTatimaStatus(string disCode, string tehCode, string villCode, string remarks, string action)
        {
            var userJson = HttpContext.Session.GetString("LoginUser");
            string userName = "";
            string IPAddress = "";
            if (!string.IsNullOrEmpty(userJson))
            {
                var user = JsonSerializer.Deserialize<LoginLog>(userJson);
                if (user != null)
                {
                    userName = user.UserName;
                    IPAddress = user.IPAddress;
                }
            }

            string Action = action.Equals("accept", StringComparison.OrdinalIgnoreCase) ? "Accepted" : "Rejected";
            var data = new VillageTatima
            {
                Dist_Code = disCode,
                Teh_Code = tehCode,
                VillageCode = villCode,
                StatusCode = (Action == "Accepted") ? 6 : 7,
                Remarks = remarks,
                IPAddress = IPAddress,
                UpdatedBy = userName
            };

            int rowsAffected = 0;

            using SqlConnection conn = new(_configuration.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new(@"sp_UpdateStatusByRevDepartment", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@DIS_CODE", data.Dist_Code);
            cmd.Parameters.AddWithValue("@TEH_CODE", data.Teh_Code);
            cmd.Parameters.AddWithValue("@Vil_CODE", data.VillageCode);
            cmd.Parameters.AddWithValue("@StatusCode", data.StatusCode);
            cmd.Parameters.AddWithValue("@IPAddress", data.IPAddress);
            cmd.Parameters.AddWithValue("@Remarks", data.Remarks);
            cmd.Parameters.AddWithValue("@UpdatedBy", data.UpdatedBy);

            conn.Open();
            rowsAffected = cmd.ExecuteNonQuery(); // ✅ Capture affected rows
            conn.Close();

            bool updateSuccessful = rowsAffected > 0;
            return Json(new { success = updateSuccessful });
        }

        [HttpPost]
        public IActionResult DownloadDocument(string distCode, string tehCode, string villCode, string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return Json(new { success = false, message = "Document not available." });
            }

            //string paddedDistCode = distCode.PadLeft(2, '0');
            //string paddedTehCode = tehCode.PadLeft(3, '0');
            //string paddedVillCode = villCode.PadLeft(5, '0');

            string filePath = Path.Combine(
                _env.WebRootPath,
                "Documents",
                distCode,
                tehCode,
                villCode,
                fileName
            );

            if (!System.IO.File.Exists(filePath))
            {
                return Json(new { success = false, message = "Document not available." });
            }

            string encodedFileName = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(fileName));

            var url = Url.Action("StartDownload", "RevDepartment", new
            {
                distCode,
                tehCode,
                villCode,
                encodedFileName
            });

            return Json(new { success = true, url });
        }

        [HttpGet]
        public IActionResult StartDownload(string distCode, string tehCode, string villCode, string encodedFileName)
        {
            if (string.IsNullOrEmpty(encodedFileName))
                return Content("Invalid filename.");

            string fileName;
            try
            {
                byte[] data = Convert.FromBase64String(encodedFileName);
                fileName = System.Text.Encoding.UTF8.GetString(data);
            }
            catch
            {
                return Content("Invalid filename format.");
            }

            //string paddedDistCode = distCode.PadLeft(2, '0');
            //string paddedTehCode = tehCode.PadLeft(3, '0');
            //string paddedVillCode = villCode.PadLeft(5, '0');

            var filePath = Path.Combine(
                _env.WebRootPath,
                "Documents",
                distCode,
                tehCode,
                villCode,
                fileName
            );

            if (!System.IO.File.Exists(filePath))
            {
                return Content("Document not available.");
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/octet-stream", fileName);
        }

        //====================== /Download File Method=======================//
    }
}