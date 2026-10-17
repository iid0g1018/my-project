using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace CampEaseApi.Controllers
{
    [ApiController]
    [Route("api/rental-items")]
    public class RentalItemsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public RentalItemsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET /api/rental-items
        // GET /api/rental-items?category=帳篷
        [HttpGet]
        public IActionResult GetRentalItems([FromQuery] string? category)
        {
            var items = new List<object>();

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"
                    SELECT
                        item_id,
                        item_name,
                        category,
                        rent_price_per_day,
                        stock_total,
                        stock_available,
                        description,
                        status
                    FROM RentalItems
                    WHERE status = 'available'
                ";

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    if (!string.IsNullOrWhiteSpace(category))
                    {
                        sql += " AND category = @category";
                        cmd.Parameters.AddWithValue("@category", category.Trim());
                    }

                    sql += " ORDER BY category, item_id";

                    cmd.CommandText = sql;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new
                            {
                                itemId = reader["item_id"],
                                itemName = reader["item_name"],
                                category = reader["category"],
                                rentPricePerDay = reader["rent_price_per_day"],
                                stockTotal = reader["stock_total"],
                                stockAvailable = reader["stock_available"],
                                description = reader["description"],
                                status = reader["status"]
                            });
                        }
                    }
                }
            }

            return Ok(items);
        }
    }
}