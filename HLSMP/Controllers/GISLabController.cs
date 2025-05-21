using HLSMP.CustomAttribute;
using HLSMP.Data;
using HLSMP.Models;
using HLSMP.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
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

        //<---------------------------District,Tehsil,Village Bind Methods---------------------->
        [HttpPost]
        public IActionResult Index(GISViewModel model)
        {
            model.DistrictList = GetDistricts();
            model.TehsilList = GetTehsils(model.DIS_CODE);
            return View(model);
        }

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
        //<---------------------------/District,Tehsil,Village Bind Methods---------------------->


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
        //<--------------------------- Upload Documnet Methods ---------------------->
 
        [HttpPost]
        public async Task<IActionResult> UploadDocument(GISViewModel model)
        {
            if (model.UploadedFile == null || model.UploadedFile.Length == 0)
                return BadRequest("No file uploaded.");

            string rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Documents");

            string folderPath = Path.Combine(rootPath, model.DIS_CODE, model.Tehsil, model.Village);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filePath = Path.Combine(folderPath, Path.GetFileName(model.UploadedFile.FileName));

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.UploadedFile.CopyToAsync(stream);
            }

            return Ok("File uploaded successfully.");
        }

        //<---------------------------/ Upload Documnet Methods ---------------------->
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

        //<---------------------------Save Method----------------------->
      public IActionResult Save(GISViewModel GISvm)
        {
            return View();
        }
        //<---------------------------/Save Method---------------------->
    }

}


