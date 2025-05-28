using HLSMP.CustomAttribute;
using HLSMP.Data;
using HLSMP.Models;
using HLSMP.Services;
using HLSMP.ViewModel;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HLSMP.Controllers
{

    //[AuthorizeRoles(1)]
    public class GISLabController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly ILocationService _locationService;

        public GISLabController(IConfiguration configuration, IWebHostEnvironment env, ILocationService locationService)
        {
            _configuration = configuration;
            _env = env;
            _locationService = locationService;


        }
        public IActionResult Index()
        { 
            //<================Get Details from session=======>
            var userJson = HttpContext.Session.GetString("LoginUser");
            int DistId = 0;
            if (!string.IsNullOrEmpty(userJson))
            {
                var user = JsonSerializer.Deserialize<LoginLog>(userJson);
                if (user != null)
                {
                    DistId = user.DistrictId;
                    
                }
            }

            var model = new GISViewModel
            {
                DistrictList = _locationService.GetDistricts() // Assuming service handles filtering

            };

            return View(model);
        }

        //<---------------------------Save Method----------------------->
        [HttpPost]
        public async Task<IActionResult> Index(GISViewModel model)
        {
            //<================Get Details from session=======>
            var userJson = HttpContext.Session.GetString("LoginUser");
            string userName = "";
            string IPAddress = "";
            int DistId = 0;
            if (!string.IsNullOrEmpty(userJson))
            {
                var user = JsonSerializer.Deserialize<LoginLog>(userJson);
                if (user != null)
                {
                    userName = user.UserName;
                    IPAddress = user.IPAddress;
                    DistId = user.DistrictId;
                }
            }
            try
            {
                model.DistrictList = _locationService.GetDistricts();
                model.TehsilList = _locationService.GetTehsils(model.DIS_CODE);
                model.VillageList = _locationService.GetVillages(model.Tehsil);
                model.VillageStageList = GetVillStageList();

                if (model.VillageStage == "3" || model.VillageStage == "4")
                {
                    // Remove validation errors for these fields
                    ModelState.Remove(nameof(model.IsWorkDone));
                    ModelState.Remove(nameof(model.WorkDate));
                    ModelState.Remove(nameof(model.UploadedFile));
                    ModelState.Remove(nameof(model.TotalTatima));
                    ModelState.Remove(nameof(model.Completed));
                    ModelState.Remove(nameof(model.Pending));
                }
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                // Validate the uploaded file
                if (model.VillageStage == "1" || model.VillageStage == "2")
                {
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

                    // Save to database
                    var data = new VillageTatima
                    {
                        Dist_Code = Convert.ToString(model.DIS_CODE),
                        Teh_Code = Convert.ToString(model.Tehsil),
                        VillageCode = Convert.ToString(model.Village),
                        TotalTatima = model.TotalTatima ?? 0,
                        Completed = model.Completed ?? 0,
                        Pending = model.Pending ?? 0,
                        IsWorkDone = model.IsWorkDone == true ? "Y" : "N",
                        UploadedDocument = savedFileName,
                        WorkDate = Convert.ToDateTime(model.WorkDate),
                        VillageStageCode = Convert.ToInt32(model.VillageStage),
                        IPAddress = IPAddress,
                        CreatedBy = userName
                    };
                    string statusCode = IsTatimaDetailExist(data);
                    if (statusCode == "1" || statusCode == "4" || statusCode == "7")
                    {
                        UpdateTatimaDetail(data);
                    }
                    else if (string.IsNullOrEmpty(statusCode))
                    {
                        InsertTatimaDetails(data);
                    }
                    else
                    {
                        TempData["AlertMessage"] = "Data already exists.";
                        return RedirectToAction("Index");
                    }
                }

                // If VillageStage is 3 or 4, skip file upload and save other details
                else
                {
                    var data = new VillageTatima
                    {
                        Dist_Code = Convert.ToString(model.DIS_CODE),
                        Teh_Code = Convert.ToString(model.Tehsil),
                        VillageCode = Convert.ToString(model.Village),
                        TotalTatima = model.TotalTatima ?? 0,
                        Completed = model.Completed ?? 0,
                        Pending = model.Pending ?? 0,
                        IsWorkDone = "",
                        UploadedDocument = "",
                        WorkDate = Convert.ToDateTime(model.WorkDate),
                        VillageStageCode = Convert.ToInt32(model.VillageStage),
                        IPAddress = IPAddress,
                        CreatedBy = userName
                    };
                    string statusCode = IsTatimaDetailExist(data);
                    if (statusCode == "1" || statusCode == "4" || statusCode == "7")
                    {
                        UpdateTatimaDetail(data);
                    }
                    else if (string.IsNullOrEmpty(statusCode))
                    {
                        InsertTatimaDetails(data);
                    }
                    else
                    {
                        TempData["AlertMessage"] = "Data already exists.";
                        return RedirectToAction("Index");
                    }

                }
                    return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = "Exception: Enter Details";
                return RedirectToAction("Index", model);
            }

        }
        private string IsTatimaDetailExist(VillageTatima data)
        {
            string StatusCode = "";
            try
            {
                using SqlConnection conn = new(_configuration.GetConnectionString("DefaultConnection"));
                using SqlCommand cmd = new(@"sp_IsTatimaDetailExist", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@Dist_Code", data.Dist_Code);
                cmd.Parameters.AddWithValue("@Teh_Code", data.Teh_Code);
                cmd.Parameters.AddWithValue("@VillageCode", data.VillageCode);

                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    StatusCode = reader["StatusCode"]?.ToString();
                }
                
                return StatusCode;
            }
            catch (Exception ex)
            { 
                return StatusCode;
            }
        }
        private void InsertTatimaDetails(VillageTatima data)
        {
            try
            {
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
                cmd.Parameters.AddWithValue("@WorkDate",
                    data.WorkDate == DateTime.MinValue ? (object)DBNull.Value : data.WorkDate);
                cmd.Parameters.AddWithValue("@VillageStageCode", data.VillageStageCode);
                cmd.Parameters.AddWithValue("@IPAddress", data.IPAddress);
                cmd.Parameters.AddWithValue("@CreatedBy", data.CreatedBy);
                cmd.Parameters.AddWithValue("@UploadedDocument", (object?)data.UploadedDocument ?? DBNull.Value);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

                TempData["Message"] = "Tatima details saved successfully.";
                TempData["Success"] = "true";
                return;
            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = "Exception: Data Already Exist";
                return;
            }
           
        }

        private void UpdateTatimaDetail(VillageTatima data)
        {
            try
            {
                using SqlConnection conn = new(_configuration.GetConnectionString("DefaultConnection"));
                using SqlCommand cmd = new(@"sp_UpdateTatimaDetailByGIS", conn)
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
                cmd.Parameters.AddWithValue("@UpdatedBy", data.CreatedBy);
                cmd.Parameters.AddWithValue("@UploadedDocument", (object?)data.UploadedDocument ?? DBNull.Value);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

                TempData["Message"] = "Tatima details Updated successfully.";
                TempData["Success"] = "true";
                return;
            }
            catch (Exception ex)
            {
                 TempData["AlertMessage"] = "Exception: Data Already Exist";
                return;
            }
        }

        //<---------------------------/Save Method---------------------->

        //<---------------------------Tehsil,Village Bind Methods---------------------->
        [HttpPost]
        public JsonResult GetTehsilsByDistrict(string DIS_CODE)
        {
            var tehsils = _locationService.GetTehsils(DIS_CODE);
            return Json(tehsils);
        }

        [HttpPost]
        public JsonResult GetVillagesByTehsil(string TehCode)
        {
            var Village = _locationService.GetVillages(TehCode);
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
        //private List<SelectListItem> GetDistricts( int DistId)
        //{
        //    var districts = new List<SelectListItem>();
        //    using SqlConnection conn = new(_configuration.GetConnectionString("DefaultConnection"));
        //    using SqlCommand cmd = new("select DIS_CODE, DIS_NAME as District from DIS_MAS where DIS_CODE= @DIS_CODE", conn);
        //    cmd.Parameters.AddWithValue("@DIS_CODE", DistId);
        //    conn.Open();
        //    SqlDataReader reader = cmd.ExecuteReader();
        //    while (reader.Read())
        //    {
        //        districts.Add(new SelectListItem
        //        {
        //            Value = reader["DIS_CODE"].ToString(),
        //            Text = reader["District"].ToString()
        //        });
        //    }
        //    return districts;
        //}

        //private List<SelectListItem> GetTehsils(string DIS_CODE)
        //{
        //    var tehsils = new List<SelectListItem>();
        //    using SqlConnection conn = new(_configuration.GetConnectionString("DefaultConnection"));
        //    using SqlCommand cmd = new("SELECT TEH_CODE, TEH_NAME FROM TEH_MAS_LGD_UPDATED WHERE DIS_CODE = @DIS_CODE", conn);
        //    cmd.Parameters.AddWithValue("@DIS_CODE", DIS_CODE);
        //    conn.Open();
        //    SqlDataReader reader = cmd.ExecuteReader();
        //    while (reader.Read())
        //    {
        //        tehsils.Add(new SelectListItem
        //        {
        //            Value = reader["TEH_CODE"].ToString(),
        //            Text = reader["TEH_NAME"].ToString()
        //        });
        //    }
        //    return tehsils;
        //}

        //private List<SelectListItem> GetVillages(string tehcode)
        //{
        //    var Village = new List<SelectListItem>();
        //    using SqlConnection conn = new(_configuration.GetConnectionString("DefaultConnection"));
        //    using SqlCommand cmd = new("SELECT VIL_NAME as Village,VIL_CODE FROM VIL_MAS WHERE TEH_CODE =@tehcode", conn);
        //    cmd.Parameters.AddWithValue("@tehcode", tehcode);
        //    conn.Open();
        //    SqlDataReader reader = cmd.ExecuteReader();
        //    while (reader.Read())
        //    {
        //        Village.Add(new SelectListItem
        //        {
        //            Value = reader["VIL_CODE"].ToString(),
        //            Text = reader["Village"].ToString()
        //        });
              
        //    }
        //    return Village;
        //}
        //<---------------------------/DropDown Bind Methods------------------->

    }

}


