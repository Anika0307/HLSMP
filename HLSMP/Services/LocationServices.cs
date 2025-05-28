using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;




namespace HLSMP.Services
{
    public interface ILocationService
    {
        List<SelectListItem> GetDistricts();
        List<SelectListItem> GetTehsils(string distCode);
        List<SelectListItem> GetVillages(string tehCode);
    }

    public class LocationService : ILocationService
    {
        private readonly IConfiguration _configuration;

        public LocationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<SelectListItem> GetDistricts() // Optional param for interface compatibility
        {
            var districts = new List<SelectListItem>();
            using SqlConnection conn = new(_configuration.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new("sp_HLSMPGetDistricts", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            // ❌ No parameters should be added here for GetDistricts
            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
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


        public List<SelectListItem> GetTehsils(string distCode)
        {
            var tehsils = new List<SelectListItem>();
            using SqlConnection conn = new(_configuration.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new("sp_HLSMPGetTehsils", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@DIS_CODE", distCode);
            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
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

        public List<SelectListItem> GetVillages(string tehCode)
        {
            var villages = new List<SelectListItem>();
            using SqlConnection conn = new(_configuration.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new("sp_HLSMPGetVillages", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@TEH_CODE", tehCode);
            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                villages.Add(new SelectListItem
                {
                    Value = reader["VIL_CODE"].ToString(),
                    Text = reader["Village"].ToString()
                });
            }
            return villages;
        }
    }
}

