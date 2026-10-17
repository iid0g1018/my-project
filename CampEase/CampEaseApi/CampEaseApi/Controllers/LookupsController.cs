using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace CampEaseApi.Controllers
{
    [ApiController]
    [Route("api/lookups")]
    public class LookupsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public LookupsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET /api/lookups/tags
        [HttpGet("tags")]
        public IActionResult GetTags()
        {
            var tags = new List<object>();

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"
                    SELECT tag_id, tag_name
                    FROM Tags
                    ORDER BY tag_id;
                ";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tags.Add(new
                        {
                            tagId = reader["tag_id"],
                            tagName = reader["tag_name"]
                        });
                    }
                }
            }

            return Ok(tags);
        }

        // GET /api/lookups/facilities
        [HttpGet("facilities")]
        public IActionResult GetFacilities()
        {
            var facilities = new List<object>();

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"
                    SELECT facility_id, facility_name
                    FROM Facilities
                    ORDER BY facility_id;
                ";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        facilities.Add(new
                        {
                            facilityId = reader["facility_id"],
                            facilityName = reader["facility_name"]
                        });
                    }
                }
            }

            return Ok(facilities);
        }
    }
}