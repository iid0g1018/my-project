using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace CampEaseApi.Controllers
{
    [ApiController]
    [Route("api/statistics")]
    public class StatisticsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public StatisticsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET /api/statistics/summary
        // 系統總覽統計
        [HttpGet("summary")]
        public IActionResult GetSummary()
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            int totalCampsites = 0;
            int totalApplications = 0;
            int approvedApplications = 0;
            int totalRentalOrders = 0;
            int totalRentalRevenue = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"
                    SELECT 
                        (SELECT COUNT(*) FROM Campsites) AS total_campsites,
                        (SELECT COUNT(*) FROM CampApplications) AS total_applications,
                        (SELECT COUNT(*) FROM CampApplications WHERE status = 'approved') AS approved_applications,
                        (SELECT COUNT(*) FROM RentalOrders) AS total_rental_orders,
                        (SELECT ISNULL(SUM(subtotal), 0) FROM RentalOrderDetails) AS total_rental_revenue;
                ";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        totalCampsites = Convert.ToInt32(reader["total_campsites"]);
                        totalApplications = Convert.ToInt32(reader["total_applications"]);
                        approvedApplications = Convert.ToInt32(reader["approved_applications"]);
                        totalRentalOrders = Convert.ToInt32(reader["total_rental_orders"]);
                        totalRentalRevenue = Convert.ToInt32(reader["total_rental_revenue"]);
                    }
                }
            }

            return Ok(new
            {
                totalCampsites = totalCampsites,
                totalApplications = totalApplications,
                approvedApplications = approvedApplications,
                totalRentalOrders = totalRentalOrders,
                totalRentalRevenue = totalRentalRevenue
            });
        }

        // GET /api/statistics/regions
        // 各地區營地數量與平均價格
        [HttpGet("regions")]
        public IActionResult GetRegionStatistics()
        {
            var result = new List<object>();

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // aggregate
                string sql = @"
                    SELECT 
                        r.county,
                        r.region_name,
                        COUNT(c.campsite_id) AS campsite_count,
                        ISNULL(AVG(c.price_per_night), 0) AS avg_price,
                        ISNULL(MIN(c.price_per_night), 0) AS min_price,
                        ISNULL(MAX(c.price_per_night), 0) AS max_price
                    FROM Regions r
                    LEFT JOIN Campsites c ON r.region_id = c.region_id
                    GROUP BY r.county, r.region_name
                    ORDER BY campsite_count DESC;
                ";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new
                        {
                            county = reader["county"],
                            regionName = reader["region_name"],
                            campsiteCount = reader["campsite_count"],
                            avgPrice = reader["avg_price"],
                            minPrice = reader["min_price"],
                            maxPrice = reader["max_price"]
                        });
                    }
                }
            }

            return Ok(result);
        }

        // GET /api/statistics/tags
        // 各標籤有幾個營地
        [HttpGet("tags")]
        public IActionResult GetTagStatistics()
        {
            var result = new List<object>();

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"
                    SELECT 
                        t.tag_name,
                        COUNT(ct.campsite_id) AS campsite_count
                    FROM Tags t
                    LEFT JOIN CampsiteTags ct ON t.tag_id = ct.tag_id
                    GROUP BY t.tag_name
                    ORDER BY campsite_count DESC;
                ";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new
                        {
                            tagName = reader["tag_name"],
                            campsiteCount = reader["campsite_count"]
                        });
                    }
                }
            }

            return Ok(result);
        }

        // GET /api/statistics/rental-popular
        // 熱門租借用品排行
        [HttpGet("rental-popular")]
        public IActionResult GetPopularRentalItems()
        {
            var result = new List<object>();

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                
                string sql = @"
                    SELECT 
                        i.item_id,
                        i.item_name,
                        i.category,
                        i.rent_price_per_day,
                        ISNULL(SUM(d.quantity), 0) AS total_quantity,
                        ISNULL(SUM(d.subtotal), 0) AS total_revenue
                    FROM RentalItems i
                    LEFT JOIN RentalOrderDetails d ON i.item_id = d.item_id
                    GROUP BY 
                        i.item_id,
                        i.item_name,
                        i.category,
                        i.rent_price_per_day
                    ORDER BY total_quantity DESC, total_revenue DESC;
                ";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new
                        {
                            itemId = reader["item_id"],
                            itemName = reader["item_name"],
                            category = reader["category"],
                            rentPricePerDay = reader["rent_price_per_day"],
                            totalQuantity = reader["total_quantity"],
                            totalRevenue = reader["total_revenue"]
                        });
                    }
                }
            }

            return Ok(result);
        }
    }
}