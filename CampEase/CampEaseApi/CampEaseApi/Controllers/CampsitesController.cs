using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace CampEaseApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CampsitesController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public CampsitesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // API 1：查營地列表，可依條件篩選
        // GET /api/campsites
        // GET /api/campsites?tag=新手友善&maxElevation=800
        [HttpGet]
        public IActionResult GetCampsites(
            [FromQuery] string? county,
            [FromQuery] string? region,
            [FromQuery] int? maxElevation,
            [FromQuery] int? maxPrice,
            [FromQuery] string? tag
        )
        {
            var campsites = new List<object>();

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"
                    SELECT DISTINCT
                        c.campsite_id,
                        c.campsite_name,
                        r.county,
                        r.region_name,
                        c.elevation,
                        c.price_per_night,
                        c.capacity,
                        c.description,
                        c.status
                    FROM Campsites c
                    JOIN Regions r ON c.region_id = r.region_id
                    WHERE c.status <> 'closed'
                ";

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    if (!string.IsNullOrWhiteSpace(county))
                    {
                        sql += " AND r.county = @county";
                        cmd.Parameters.AddWithValue("@county", county.Trim());
                    }

                    if (!string.IsNullOrWhiteSpace(region))
                    {
                        sql += " AND r.region_name = @region";
                        cmd.Parameters.AddWithValue("@region", region.Trim());
                    }

                    if (maxElevation.HasValue)
                    {
                        sql += " AND c.elevation <= @maxElevation";
                        cmd.Parameters.AddWithValue("@maxElevation", maxElevation.Value);
                    }

                    if (maxPrice.HasValue)
                    {
                        sql += " AND c.price_per_night <= @maxPrice";
                        cmd.Parameters.AddWithValue("@maxPrice", maxPrice.Value);
                    }

                    if (!string.IsNullOrWhiteSpace(tag))
                    {
                        sql += @"
                            AND c.campsite_id IN (
                                SELECT ct.campsite_id
                                FROM CampsiteTags ct
                                JOIN Tags t ON ct.tag_id = t.tag_id
                                WHERE t.tag_name = @tag
                            )
                        ";
                        cmd.Parameters.AddWithValue("@tag", tag.Trim());
                    }

                    sql += " ORDER BY c.campsite_id";

                    cmd.CommandText = sql;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            campsites.Add(new
                            {
                                campsiteId = reader["campsite_id"],
                                campsiteName = reader["campsite_name"],
                                county = reader["county"],
                                regionName = reader["region_name"],
                                elevation = reader["elevation"],
                                pricePerNight = reader["price_per_night"],
                                capacity = reader["capacity"],
                                description = reader["description"],
                                status = reader["status"]
                            });
                        }
                    }
                }
            }

            return Ok(campsites);
        }

        // API 2：查單一營地詳細資料
        // GET /api/campsites/1
        [HttpGet("{id}")]
        public IActionResult GetCampsiteDetail(int id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            object? campsite = null;
            var tags = new List<string>();
            var facilities = new List<object>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string campsiteSql = @"
                    SELECT
                        c.campsite_id,
                        c.campsite_name,
                        r.county,
                        r.region_name,
                        c.address,
                        c.elevation,
                        c.price_per_night,
                        c.capacity,
                        c.description,
                        c.status
                    FROM Campsites c
                    JOIN Regions r ON c.region_id = r.region_id
                    WHERE c.campsite_id = @id;
                ";

                using (SqlCommand cmd = new SqlCommand(campsiteSql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            campsite = new
                            {
                                campsiteId = reader["campsite_id"],
                                campsiteName = reader["campsite_name"],
                                county = reader["county"],
                                regionName = reader["region_name"],
                                address = reader["address"],
                                elevation = reader["elevation"],
                                pricePerNight = reader["price_per_night"],
                                capacity = reader["capacity"],
                                description = reader["description"],
                                status = reader["status"]
                            };
                        }
                        else
                        {
                            return NotFound(new
                            {
                                message = "找不到這個營地"
                            });
                        }
                    }
                }

                string tagSql = @"
                    SELECT t.tag_name
                    FROM CampsiteTags ct
                    JOIN Tags t ON ct.tag_id = t.tag_id
                    WHERE ct.campsite_id = @id;
                ";

                using (SqlCommand cmd = new SqlCommand(tagSql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tags.Add(reader["tag_name"].ToString());
                        }
                    }
                }

                string facilitySql = @"
                    SELECT
                        f.facility_name,
                        cf.quantity,
                        cf.note
                    FROM CampsiteFacilities cf
                    JOIN Facilities f ON cf.facility_id = f.facility_id
                    WHERE cf.campsite_id = @id;
                ";

                using (SqlCommand cmd = new SqlCommand(facilitySql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            facilities.Add(new
                            {
                                facilityName = reader["facility_name"],
                                quantity = reader["quantity"],
                                note = reader["note"]
                            });
                        }
                    }
                }
            }

            return Ok(new
            {
                campsite = campsite,
                tags = tags,
                facilities = facilities
            });
        }

        // API 3：新增營地
        // POST /api/campsites
        [HttpPost]
        public IActionResult CreateCampsite([FromBody] CreateCampsiteRequest request)
        {
            if (request.RegionId <= 0)
            {
                return BadRequest(new { message = "regionId 不可為空" });
            }

            if (string.IsNullOrWhiteSpace(request.CampsiteName))
            {
                return BadRequest(new { message = "營地名稱不可為空" });
            }

            if (request.Elevation < 0)
            {
                return BadRequest(new { message = "海拔不可小於 0" });
            }

            if (request.PricePerNight <= 0)
            {
                return BadRequest(new { message = "價格必須大於 0" });
            }

            if (request.Capacity <= 0)
            {
                return BadRequest(new { message = "容納人數必須大於 0" });
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            int newCampsiteId;

            using (SqlConnection conn = new SqlConnection(connectionString))

            {

                conn.Open();

                string sql = @"

                    INSERT INTO Campsites

                    (region_id, campsite_name, address, elevation, price_per_night, capacity, description, status)

                    VALUES

                    (@regionId, @campsiteName, @address, @elevation, @pricePerNight, @capacity, @description, 'available');

                    SELECT SCOPE_IDENTITY();

                ";

                using (SqlCommand cmd = new SqlCommand(sql, conn))

                {

                    cmd.Parameters.AddWithValue("@regionId", request.RegionId);

                    cmd.Parameters.AddWithValue("@campsiteName", request.CampsiteName.Trim());

                    cmd.Parameters.AddWithValue("@address", request.Address ?? "");

                    cmd.Parameters.AddWithValue("@elevation", request.Elevation);

                    cmd.Parameters.AddWithValue("@pricePerNight", request.PricePerNight);

                    cmd.Parameters.AddWithValue("@capacity", request.Capacity);

                    cmd.Parameters.AddWithValue("@description", request.Description ?? "");

                    newCampsiteId = Convert.ToInt32(cmd.ExecuteScalar());

                }

            }

            return Ok(new

            {

                message = "營地新增成功",

                campsiteId = newCampsiteId

            });

        }

        // API 4：修改營地

        // PUT /api/campsites/1

        [HttpPut("{id}")]

        public IActionResult UpdateCampsite(int id, [FromBody] UpdateCampsiteRequest request)

        {

            if (id <= 0)

            {

                return BadRequest(new { message = "營地 ID 不正確" });

            }

            if (request.RegionId <= 0)

            {

                return BadRequest(new { message = "regionId 不可為空" });

            }

            if (string.IsNullOrWhiteSpace(request.CampsiteName))

            {

                return BadRequest(new { message = "營地名稱不可為空" });

            }

            if (request.PricePerNight <= 0)

            {

                return BadRequest(new { message = "價格必須大於 0" });

            }

            if (request.Capacity <= 0)

            {

                return BadRequest(new { message = "容納人數必須大於 0" });

            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            int rowsAffected;

            using (SqlConnection conn = new SqlConnection(connectionString))

            {

                conn.Open();

                string sql = @"

                    UPDATE Campsites

                    SET

                        region_id = @regionId,

                        campsite_name = @campsiteName,

                        address = @address,

                        elevation = @elevation,

                        price_per_night = @pricePerNight,

                        capacity = @capacity,

                        description = @description,

                        status = @status

                    WHERE campsite_id = @id;

                ";

                using (SqlCommand cmd = new SqlCommand(sql, conn))

                {

                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.Parameters.AddWithValue("@regionId", request.RegionId);

                    cmd.Parameters.AddWithValue("@campsiteName", request.CampsiteName.Trim());

                    cmd.Parameters.AddWithValue("@address", request.Address ?? "");

                    cmd.Parameters.AddWithValue("@elevation", request.Elevation);

                    cmd.Parameters.AddWithValue("@pricePerNight", request.PricePerNight);

                    cmd.Parameters.AddWithValue("@capacity", request.Capacity);

                    cmd.Parameters.AddWithValue("@description", request.Description ?? "");

                    cmd.Parameters.AddWithValue("@status", request.Status ?? "available");

                    rowsAffected = cmd.ExecuteNonQuery();

                }

            }

            if (rowsAffected == 0)

            {

                return NotFound(new

                {

                    message = "找不到這個營地"

                });

            }

            return Ok(new

            {

                message = "營地修改成功",

                campsiteId = id

            });

        }

        // API 5：刪除 / 下架營地

        // DELETE /api/campsites/1

        [HttpDelete("{id}")]

        public IActionResult DeleteCampsite(int id)

        {

            if (id <= 0)

            {

                return BadRequest(new { message = "營地 ID 不正確" });

            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            int rowsAffected;

            using (SqlConnection conn = new SqlConnection(connectionString))

            {

                conn.Open();

                string sql = @"

                    UPDATE Campsites

                    SET status = 'closed'

                    WHERE campsite_id = @id;

                ";

                using (SqlCommand cmd = new SqlCommand(sql, conn))

                {

                    cmd.Parameters.AddWithValue("@id", id);

                    rowsAffected = cmd.ExecuteNonQuery();

                }

            }

            if (rowsAffected == 0)

            {

                return NotFound(new

                {

                    message = "找不到這個營地"

                });

            }

            return Ok(new

            {

                message = "營地已下架",

                campsiteId = id

            });

        }

    }

    public class CreateCampsiteRequest

    {

        public int RegionId { get; set; }

        public string CampsiteName { get; set; } = "";

        public string? Address { get; set; }

        public int Elevation { get; set; }

        public int PricePerNight { get; set; }

        public int Capacity { get; set; }

        public string? Description { get; set; }

    }

    public class UpdateCampsiteRequest

    {

        public int RegionId { get; set; }

        public string CampsiteName { get; set; } = "";

        public string? Address { get; set; }

        public int Elevation { get; set; }

        public int PricePerNight { get; set; }

        public int Capacity { get; set; }

        public string? Description { get; set; }

        public string? Status { get; set; }
    }
}