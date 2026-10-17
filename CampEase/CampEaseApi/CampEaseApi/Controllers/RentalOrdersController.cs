using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace CampEaseApi.Controllers
{
    [ApiController]
    [Route("api/rental-orders")]
    public class RentalOrdersController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public RentalOrdersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET /api/rental-orders
        // 查詢所有租借紀錄
        [HttpGet]
        public IActionResult GetRentalOrders()
        {
            var orders = new List<object>();

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"
                    SELECT
                        o.rental_order_id,
                        u.name AS user_name,
                        o.start_date,
                        o.end_date,
                        o.status,
                        i.item_name,
                        d.quantity,
                        d.subtotal,
                        o.created_at
                    FROM RentalOrders o
                    JOIN Users u ON o.user_id = u.user_id
                    JOIN RentalOrderDetails d ON o.rental_order_id = d.rental_order_id
                    JOIN RentalItems i ON d.item_id = i.item_id
                    ORDER BY o.created_at DESC;
                ";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        orders.Add(new
                        {
                            rentalOrderId = reader["rental_order_id"],
                            userName = reader["user_name"],
                            startDate = reader["start_date"],
                            endDate = reader["end_date"],
                            status = reader["status"],
                            itemName = reader["item_name"],
                            quantity = reader["quantity"],
                            subtotal = reader["subtotal"],
                            createdAt = reader["created_at"]
                        });
                    }
                }
            }

            return Ok(orders);
        }

        // POST /api/rental-orders
        // 新增租借訂單
        [HttpPost]
        public IActionResult CreateRentalOrder([FromBody] CreateRentalOrderRequest request)
        {
            if (request.UserId <= 0)
            {
                return BadRequest(new { message = "userId 不可為空" });
            }

            if (request.EndDate <= request.StartDate)
            {
                return BadRequest(new { message = "歸還日期必須晚於租借日期" });
            }

            if (request.Items == null || request.Items.Count == 0)
            {
                return BadRequest(new { message = "至少要選擇一項租借用品" });
            }

            int rentalDays = (request.EndDate.Date - request.StartDate.Date).Days;

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            int newRentalOrderId;
            int totalAmount = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string orderSql = @"
                            INSERT INTO RentalOrders
                            (user_id, start_date, end_date, status)
                            VALUES
                            (@userId, @startDate, @endDate, 'pending');

                            SELECT SCOPE_IDENTITY();
                        ";

                        using (SqlCommand cmd = new SqlCommand(orderSql, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@userId", request.UserId);
                            cmd.Parameters.AddWithValue("@startDate", request.StartDate);
                            cmd.Parameters.AddWithValue("@endDate", request.EndDate);

                            newRentalOrderId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        foreach (var item in request.Items)
                        {
                            if (item.Quantity <= 0)
                            {
                                throw new Exception("租借數量必須大於 0");
                            }

                            int pricePerDay = 0;
                            int stockAvailable = 0;

                            string checkItemSql = @"
                                SELECT rent_price_per_day, stock_available
                                FROM RentalItems
                                WHERE item_id = @itemId
                                AND status = 'available';
                            ";

                            using (SqlCommand cmd = new SqlCommand(checkItemSql, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@itemId", item.ItemId);

                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        pricePerDay = Convert.ToInt32(reader["rent_price_per_day"]);
                                        stockAvailable = Convert.ToInt32(reader["stock_available"]);
                                    }
                                    else
                                    {
                                        throw new Exception("找不到租借用品，itemId：" + item.ItemId);
                                    }
                                }
                            }

                            if (stockAvailable < item.Quantity)
                            {
                                throw new Exception("用品庫存不足，itemId：" + item.ItemId);
                            }

                            int subtotal = pricePerDay * item.Quantity * rentalDays;
                            totalAmount += subtotal;

                            string detailSql = @"
                                INSERT INTO RentalOrderDetails
                                (rental_order_id, item_id, quantity, subtotal)
                                VALUES
                                (@rentalOrderId, @itemId, @quantity, @subtotal);
                            ";

                            using (SqlCommand cmd = new SqlCommand(detailSql, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@rentalOrderId", newRentalOrderId);
                                cmd.Parameters.AddWithValue("@itemId", item.ItemId);
                                cmd.Parameters.AddWithValue("@quantity", item.Quantity);
                                cmd.Parameters.AddWithValue("@subtotal", subtotal);

                                cmd.ExecuteNonQuery();
                            }

                            string updateStockSql = @"
                                UPDATE RentalItems
                                SET stock_available = stock_available - @quantity
                                WHERE item_id = @itemId;
                            ";

                            using (SqlCommand cmd = new SqlCommand(updateStockSql, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@quantity", item.Quantity);
                                cmd.Parameters.AddWithValue("@itemId", item.ItemId);

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
                            message = ex.Message
                        });
                    }
                }
            }

            return Ok(new
            {
                message = "露營用品租借訂單建立成功",
                rentalOrderId = newRentalOrderId,
                rentalDays = rentalDays,
                totalAmount = totalAmount
            });
        }
    }

    public class CreateRentalOrderRequest
    {
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<RentalOrderItemRequest> Items { get; set; } = new List<RentalOrderItemRequest>();
    }

    public class RentalOrderItemRequest
    {
        public int ItemId { get; set; }
        public int Quantity { get; set; }
    }
}