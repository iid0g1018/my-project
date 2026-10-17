using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace CampEaseApi.Controllers
{
    [ApiController]
    [Route("api/campsites")]
    public class CampsiteRelationsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public CampsiteRelationsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // PUT /api/campsites/1/tags
        // 修改某營地的標籤
        [HttpPut("{id}/tags")]
        public IActionResult UpdateCampsiteTags(int id, [FromBody] UpdateCampsiteTagsRequest request)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "營地 ID 不正確" });
            }

            if (request.TagIds == null)
            {
                return BadRequest(new { message = "tagIds 不可為空" });
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 先確認營地存在
                        string checkSql = @"
                            SELECT COUNT(*)
                            FROM Campsites
                            WHERE campsite_id = @id;
                        ";

                        using (SqlCommand cmd = new SqlCommand(checkSql, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@id", id);
                            int count = Convert.ToInt32(cmd.ExecuteScalar());

                            if (count == 0)
                            {
                                transaction.Rollback();
                                return NotFound(new { message = "找不到這個營地" });
                            }
                        }

                        // 先刪除原本標籤
                        string deleteSql = @"
                            DELETE FROM CampsiteTags
                            WHERE campsite_id = @id;
                        ";

                        using (SqlCommand cmd = new SqlCommand(deleteSql, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.ExecuteNonQuery();
                        }

                        // 再新增新的標籤
                        foreach (int tagId in request.TagIds.Distinct())
                        {
                            string insertSql = @"
                                INSERT INTO CampsiteTags (campsite_id, tag_id)
                                VALUES (@campsiteId, @tagId);
                            ";

                            using (SqlCommand cmd = new SqlCommand(insertSql, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@campsiteId", id);
                                cmd.Parameters.AddWithValue("@tagId", tagId);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        return BadRequest(new
                        {
                            message = "更新營地標籤失敗",
                            error = ex.Message
                        });
                    }
                }
            }

            return Ok(new
            {
                message = "營地標籤更新成功",
                campsiteId = id,
                tagIds = request.TagIds
            });
        }

        // PUT /api/campsites/1/facilities
        // 修改某營地的設備
        [HttpPut("{id}/facilities")]
        public IActionResult UpdateCampsiteFacilities(int id, [FromBody] UpdateCampsiteFacilitiesRequest request)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "營地 ID 不正確" });
            }

            if (request.Facilities == null)
            {
                return BadRequest(new { message = "facilities 不可為空" });
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 先確認營地存在
                        string checkSql = @"
                            SELECT COUNT(*)
                            FROM Campsites
                            WHERE campsite_id = @id;
                        ";

                        using (SqlCommand cmd = new SqlCommand(checkSql, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@id", id);
                            int count = Convert.ToInt32(cmd.ExecuteScalar());

                            if (count == 0)
                            {
                                transaction.Rollback();
                                return NotFound(new { message = "找不到這個營地" });
                            }
                        }

                        // 先刪除原本設備
                        string deleteSql = @"
                            DELETE FROM CampsiteFacilities
                            WHERE campsite_id = @id;
                        ";

                        using (SqlCommand cmd = new SqlCommand(deleteSql, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.ExecuteNonQuery();
                        }

                        // 再新增新的設備
                        foreach (var facility in request.Facilities)
                        {
                            if (facility.FacilityId <= 0)
                            {
                                throw new Exception("facilityId 不正確");
                            }

                            if (facility.Quantity <= 0)
                            {
                                throw new Exception("設備數量必須大於 0");
                            }

                            string insertSql = @"
                                INSERT INTO CampsiteFacilities
                                (campsite_id, facility_id, quantity, note)
                                VALUES
                                (@campsiteId, @facilityId, @quantity, @note);
                            ";

                            using (SqlCommand cmd = new SqlCommand(insertSql, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@campsiteId", id);
                                cmd.Parameters.AddWithValue("@facilityId", facility.FacilityId);
                                cmd.Parameters.AddWithValue("@quantity", facility.Quantity);
                                cmd.Parameters.AddWithValue("@note", facility.Note ?? "");
                                cmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        return BadRequest(new
                        {
                            message = "更新營地設備失敗",
                            error = ex.Message
                        });
                    }
                }
            }

            return Ok(new
            {
                message = "營地設備更新成功",
                campsiteId = id
            });
        }
    }

    public class UpdateCampsiteTagsRequest
    {
        public List<int> TagIds { get; set; } = new List<int>();
    }

    public class UpdateCampsiteFacilitiesRequest
    {
        public List<CampsiteFacilityRequest> Facilities { get; set; } = new List<CampsiteFacilityRequest>();
    }

    public class CampsiteFacilityRequest
    {
        public int FacilityId { get; set; }
        public int Quantity { get; set; }
        public string? Note { get; set; }
    }
}