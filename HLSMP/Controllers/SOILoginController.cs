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

            var model1 = new SOIVillages();
            using SqlConnection conn = new(_configuration.GetConnectionString("DefaultConnection"));
            string query = @" SELECT vil.VIL_NAME AS  VillageName,v.TotalTatima,v.Completed,v.Pending,v.IsWorkDone,v.UploadedDocument,v.WorkDate,r.Reason AS VillageStage from   VillageTatimas v left JOIN TblReason_MAS r ON v.VillageStageCode = r.ReasonId INNER JOIN VIL_MAS vil ON vil.VIL_CODE = v.VillageCode";

            using SqlCommand cmd = new(query, conn);
            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                model.Villages.Add(new SOIVillages
                {
                    VillageName = reader["VillageName"].ToString(),
                    TotalTatima = Convert.ToInt32(reader["TotalTatima"]),
                    CompletedTatima = Convert.ToInt32(reader["Completed"]),
                    PendingTatima = Convert.ToInt32(reader["Pending"]),
                    IsWorkDone = reader["IsWorkDone"].ToString() == "Y" ? "Yes" : "No",
                    //UploadedFile = reader["UploadedDocument"]?.,
                    WorkDate = Convert.ToDateTime(reader["WorkDate"]),
                    //VillageStageCode = reader["VillageStage"].ToString()
                });
            }

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

           
            string query = @"
        SELECT vil.VIL_NAME AS VillageName,v.VillageCode, v.TotalTatima, v.Completed, v.Pending, v.IsWorkDone,v.UploadedDocument, v.WorkDate, r.Reason AS VillageStage FROM VillageTatimas v LEFT JOIN TblReason_MAS r ON v.VillageStageCode = r.ReasonId INNER JOIN VIL_MAS vil ON vil.VIL_CODE = v.VillageCode WHERE Dist_Code = @DIS_CODE AND v.Teh_Code = @TEH_CODE";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@DIS_CODE", DIS_CODE);
            cmd.Parameters.AddWithValue("@TEH_CODE", TEH_CODE);

            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if (villages == null)
                {
                    villages = new List<SOIVillages>();
                }

                string villageCode = reader["VillageCode"] != DBNull.Value
    ? reader["VillageCode"].ToString()
    : string.Empty;

                villages.Add(new SOIVillages
                {
                    VillageName = reader["VillageName"] != DBNull.Value
                        ? reader["VillageName"].ToString()
                        : null,



                    TotalTatima = reader["TotalTatima"] != DBNull.Value
                        ? Convert.ToInt32(reader["TotalTatima"])
                        : 0,

                    CompletedTatima = reader["Completed"] != DBNull.Value
                        ? Convert.ToInt32(reader["Completed"])
                        : 0,

                    PendingTatima = reader["Pending"] != DBNull.Value
                        ? Convert.ToInt32(reader["Pending"])
                        : 0,

                    IsWorkDone = reader["IsWorkDone"] != DBNull.Value
                        ? (reader["IsWorkDone"].ToString() == "Y" ? "Yes" : "No")
                        : null,

                    UploadedFile = reader["UploadedDocument"] != DBNull.Value && !string.IsNullOrEmpty(villageCode)
    ? $"/Documents/{DIS_CODE}/{TEH_CODE}/{villageCode}/{(reader["UploadedDocument"])}"
    : null,



                    WorkDate = reader["WorkDate"] != DBNull.Value
                        ? Convert.ToDateTime(reader["WorkDate"])
                        : (DateTime?)null,

                    VillageStage = reader["VillageStage"] != DBNull.Value
                        ? reader["VillageStage"].ToString()
                        : null
                });
            }

            return villages;
        }

    }
}
           
        
