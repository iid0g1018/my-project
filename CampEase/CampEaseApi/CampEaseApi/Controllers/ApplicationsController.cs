using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace CampEaseApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ApplicationsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // API 1：查詢所有營地申請紀錄
        // GET /api/applications
        [HttpGet]
        public IActionResult GetApplications()
        {
            var applications = new List<object>();

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"
                    SELECT
                        a.application_id,
                        u.name AS user_name,
                        c.campsite_name,
                        a.start_date,
                        a.end_date,
                        a.people_count,
                        a.status,
                        a.note,
                        a.created_at
                    FROM CampApplications a
                    JOIN Users u ON a.user_id = u.user_id
                    JOIN Campsites c ON a.campsite_id = c.campsite_id
                    ORDER BY a.created_at DESC;
                ";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        applications.Add(new
                        {
                            applicationId = reader["application_id"],
                            userName = reader["user_name"],
                            campsiteName = reader["campsite_name"],
                            startDate = reader["start_date"],
                            endDate = reader["end_date"],
                            peopleCount = reader["people_count"],
                            status = reader["status"],
                            note = reader["note"],
                            createdAt = reader["created_at"]
                        });
                    }
                }
            }

            return Ok(applications);
        }

        // API 2：新增營地申請
        // POST /api/applications
        [HttpPost]
        public IActionResult CreateApplication([FromBody] CreateApplicationRequest request)
        {
            if (request.UserId <= 0)
            {
                return BadRequest(new { message = "userId 不可為空" });
            }

            if (request.CampsiteId <= 0)
            {
                return BadRequest(new { message = "campsiteId 不可為空" });
            }

            if (request.PeopleCount <= 0)
            {
                return BadRequest(new { message = "人數必須大於 0" });
            }

            if (request.EndDate <= request.StartDate)
            {
                return BadRequest(new { message = "離開日期必須晚於入住日期" });
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            int newApplicationId;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"
                    INSERT INTO CampApplications
                    (user_id, campsite_id, start_date, end_date, people_count, status, note)
                    VALUES
                    (@userId, @campsiteId, @startDate, @endDate, @peopleCount, 'pending', @note);

                    SELECT SCOPE_IDENTITY();
                ";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", request.UserId);
                    cmd.Parameters.AddWithValue("@campsiteId", request.CampsiteId);
                    cmd.Parameters.AddWithValue("@startDate", request.StartDate);
                    cmd.Parameters.AddWithValue("@endDate", request.EndDate);
                    cmd.Parameters.AddWithValue("@peopleCount", request.PeopleCount);
                    cmd.Parameters.AddWithValue("@note", request.Note ?? "");

                    newApplicationId = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            return Ok(new
            {
                message = "營地申請建立成功",
                applicationId = newApplicationId
            });
        }

        // API 3：管理員審核營地申請
        // PUT /api/applications/1/status
        [HttpPut("{id}/status")]
        public IActionResult UpdateApplicationStatus(int id, [FromBody] UpdateApplicationStatusRequest request)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "申請 ID 不正確" });
            }

            if (string.IsNullOrWhiteSpace(request.Status))
            {
                return BadRequest(new { message = "status 不可為空" });
            }

            string status = request.Status.Trim().ToLower();

            string[] allowedStatuses = { "pending", "approved", "rejected", "cancelled" };

            if (!allowedStatuses.Contains(status))
            {
                return BadRequest(new
                {
                    message = "狀態只能是 pending、approved、rejected、cancelled"
                });
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            int rowsAffected;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"
                    UPDATE CampApplications
                    SET status = @status
                    WHERE application_id = @id;
                ";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@id", id);

                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }

            if (rowsAffected == 0)
            {
                return NotFound(new
                {
                    message = "找不到這筆營地申請"
                });
            }

            return Ok(new
            {
                message = "營地申請狀態更新成功",
                applicationId = id,
                status = status
            });
        }
    }

    public class CreateApplicationRequest
    {
        public int UserId { get; set; }
        public int CampsiteId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PeopleCount { get; set; }
        public string? Note { get; set; }
    }

    public class UpdateApplicationStatusRequest
    {
        public string Status { get; set; } = "";
    }
}