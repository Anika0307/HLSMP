using HLSMP.CustomAttribute;
using HLSMP.Data;
using HLSMP.Models;
using HLSMP.ViewModel;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

namespace HLSMP.Controllers
{

    [AuthorizeRoles(1)]
    public class GISLabController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public GISLabController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }
        public IActionResult Index()
        {
            var model = new GISViewModel
            {
                DistrictList = GetDistricts()
            };

            return View(model);
        }

        //<---------------------------Save Method----------------------->
        [HttpPost]
        public async Task<IActionResult> Index(GISViewModel model)
        {
            try
            {
                model.DistrictList = GetDistricts();
                model.TehsilList = GetTehsils(model.DIS_CODE);
                model.VillageList = GetVillages(model.Tehsil);
                model.VillageStageList = GetVillStageList();

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                string rootPath = Path.Combine(_env.WebRootPath, "Documents", model.DIS_CODE, model.Tehsil, model.Village);
                if (!Directory.Exists(rootPath))
                    Directory.CreateDirectory(rootPath);

                string savedFileName = null;
                if (model.UploadedFile != null && model.UploadedFile.Length > 0)
                {
                    savedFileName = Path.GetFileName(model.UploadedFile.FileName);
                    string fullPath = Path.Combine(rootPath, savedFileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await model.UploadedFile.CopyToAsync(stream);
                    }
                }
                //<================Get Details from session=======>
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

                // Save to database
                var data = new VillageTatima
                {
                    Dist_Code = int.Parse(model.DIS_CODE),
                    Teh_Code = int.Parse(model.Tehsil),
                    VillageCode = int.Parse(model.Village),
                    TotalTatima = model.TotalTatima,
                    Completed = model.Completed,
                    Pending = model.Pending,
                    IsWorkDone = model.IsWorkDone == true ? "Y" : "N",
                    UploadedDocument = savedFileName,
                    WorkDate = Convert.ToDateTime(model.WorkDate),
                    VillageStageCode = Convert.ToInt32(model.VillageStage),
                    IPAddress= IPAddress,
                    CreatedBy = userName
                };

                using SqlConnection conn = new(_configuration.GetConnectionString("DefaultConnection"));
                using SqlCommand cmd = new(@"sp_InsertTatimaDetailByGIS", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@Dist_Code", data.Dist_Code);
                cmd.Parameters.AddWithValue("@Teh_Code", data.Teh_Code);
                cmd.Parameters.AddWithValue("@VillageCode", data.VillageCode);
                cmd.Parameters.AddWithValue("@TotalTatima", data.TotalTatima);
                cmd.Parameters.AddWithValue("@Completed", data.Completed);
                cmd.Parameters.AddWithValue("@Pending", data.Pending);
                cmd.Parameters.AddWithValue("@IsWorkDone", data.IsWorkDone);
                cmd.Parameters.AddWithValue("@WorkDate", data.WorkDate);
                cmd.Parameters.AddWithValue("@VillageStageCode", data.VillageStageCode);
                cmd.Parameters.AddWithValue("@IPAddress", data.IPAddress);
                cmd.Parameters.AddWithValue("@CreatedBy", data.CreatedBy);
                cmd.Parameters.AddWithValue("@UploadedDocument", (object?)data.UploadedDocument ?? DBNull.Value);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

                TempData["Message"] = "Tatima details saved successfully.";
                TempData["Success"] = "true";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = "Exception: Enter Details";
                return RedirectToAction("Index", model);
            }

        }
        //<---------------------------/Save Method---------------------->

        //<---------------------------Tehsil,Village Bind Methods---------------------->
        [HttpPost]
        public JsonResult GetTehsilsByDistrict(string DIS_CODE)
        {
            var tehsils = GetTehsils(DIS_CODE);
            return Json(tehsils);
        }

        [HttpPost]
        public JsonResult GetVillagesByTehsil(string TehCode)
        {
            var Village = GetVillages(TehCode);
            return Json(Village);
        }
        //<---------------------------/Tehsil,Village Bind Methods---------------------->

        //<---------------------------Reason Bind Methods---------------------->
        public JsonResult GetVillageStageList()
        {
            var VillageStageList = GetVillStageList();
            return Json(VillageStageList);
        }
        private List<SelectListItem> GetVillStageList()
        {
            var Reasons = new List<SelectListItem>();
            using SqlConnection conn = new(_configuration.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new("SELECT ReasonId ,Reason FROM TblReason_MAS", conn);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Reasons.Add(new SelectListItem
                {
                    Value = reader["ReasonId"].ToString(),
                    Text = reader["Reason"].ToString()
                });
            }
            conn.Close();
            return Reasons;
        }

        //<---------------------------/Reason Bind Methods ---------------------->
       
        //<---------------------------DropDown Bind Methods---------------------->
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
            using SqlCommand cmd = new("SELECT TEH_CODE, TEH_NAME FROM TEH_MAS_LGD_UPDATED WHERE DIS_CODE = @DIS_CODE", conn);
            cmd.Parameters.AddWithValue("@DIS_CODE", DIS_CODE);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                tehsils.Add(new SelectListItem
                {
                    Value = reader["TEH_CODE"].ToString(),
                    Text = reader["TEH_NAME"].ToString()
                });
            }
            return tehsils;
        }

        private List<SelectListItem> GetVillages(string tehcode)
        {
            var Village = new List<SelectListItem>();
            using SqlConnection conn = new(_configuration.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new("SELECT VIL_NAME as Village,VIL_CODE FROM VIL_MAS WHERE TEH_CODE =@tehcode", conn);
            cmd.Parameters.AddWithValue("@tehcode", tehcode);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Village.Add(new SelectListItem
                {
                    Value = reader["VIL_CODE"].ToString(),
                    Text = reader["Village"].ToString()
                });
              
            }
            return Village;
        }
        //<---------------------------/DropDown Bind Methods------------------->

    }

}


