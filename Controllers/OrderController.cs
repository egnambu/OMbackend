using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMbackend.Data;
using OMbackend.Models;
using System.ComponentModel.DataAnnotations;

namespace OMbackend.Controllers
{
    [Authorize(AuthenticationSchemes = "CookieAuth", Policy = "UserPolicy")]
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("UsersOrder/{userId}")]
        public async Task<ActionResult> GetOrdersByUserId(int userId)
        {
            // Check if the user exists
            var userExists = await _context.Users.AnyAsync(u => u.ID == userId);
            if (!userExists)
            {
                return NotFound($"User with ID {userId} not found.");
            }

            // Query orders along with shop details
            var ordersWithShops = await _context.Orders
                .Where(o => o.UserID == userId)
                .Include(o => o.Shop) // Load related Shop entity
                .Select(o => new
                {
                    o.ID,
                    o.ShopID,
                    o.Title,
                    o.Price,
                    o.CreatedAt,
                    o.Status,
                    ShopDetails = o.Shop == null ? null : new
                    {
                        o.Shop.ID,
                        o.Shop.Name,
                        o.Shop.Heading,
                        o.Shop.Description,
                        o.Shop.Trust,
                        o.Shop.Reviews,
                        o.Shop.IconsIndex,
                        o.Shop.Services,
                        o.Shop.Image,
                        o.Shop.Ratings,
                        o.Shop.Multiplier,
                        o.Shop.IsAvailable
                    }
                })
                .ToListAsync();

            // Check if there are no orders
            if (!ordersWithShops.Any())
            {
                return NotFound($"No orders found for User ID {userId}");
            }

            return Ok(ordersWithShops);
        }



        [HttpPost("create")]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] Order order)
        {
            if (order == null)
            {
                return BadRequest("Order data is required.");
            }

            try
            {
                // Validate User exists
                var userExists = await _context.Users.AnyAsync(u => u.ID == order.UserID);
                if (!userExists)
                {
                    return BadRequest($"User with ID {order.UserID} does not exist.");
                }

                // Validate Shop exists
                var shopExists = await _context.Shops.AnyAsync(s => s.ID == order.ShopID);
                if (!shopExists)
                {
                    return BadRequest($"Shop with ID {order.ShopID} does not exist.");
                }

                // Set CreatedAt to current time
                order.CreatedAt = DateTime.UtcNow;

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetOrdersByUserId), new { userId = order.UserID }, order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating the order: {ex.Message}");
            }
        }


        [HttpPut("UpdatePaymentReference/{orderId}")]
        public async Task<IActionResult> UpdatePaymentReference(int orderId, [FromBody] string newPaymentReference)
        {
            if (string.IsNullOrWhiteSpace(newPaymentReference))
            {
                return BadRequest("PaymentReference cannot be null or empty.");
            }

            // Find the order by ID
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound($"Order with ID {orderId} not found.");
            }

            // Update the PaymentReference
            order.PaymentReference = newPaymentReference;

            try
            {
                // Save the changes to the database
                await _context.SaveChangesAsync();
                return Ok($"PaymentReference for Order ID {orderId} updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the PaymentReference: {ex.Message}");
            }
        }



        [HttpPut("updateIsPaid/{orderId}")]
        public async Task<ActionResult<Order>> UpdateIsPaid(int orderId, [FromBody] bool isPaid)
        {
            // Find the order by ID
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound($"Order with ID {orderId} not found.");
            }

            // Update the isPaid status
            order.isPaid = isPaid;

            try
            {
                // Save the changes to the database
                await _context.SaveChangesAsync();
                return Ok(order); // Return the updated order
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the order: {ex.Message}");
            }
        }


        [HttpDelete("delete/{orderId}")]
        public async Task<ActionResult> DeleteOrder(int orderId)
        {
            // Find the order by ID
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound($"Order with ID {orderId} not found.");
            }

            try
            {
                // Remove the order from the database
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();

                return NoContent(); // Return 204 No Content, indicating successful deletion
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the order: {ex.Message}");
            }
        }


        [HttpGet("all-orders")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders()
        {
            try
            {
                var orders = await _context.Orders
                    .Include(o => o.User) // Include the related User entity
                    .Select(o => new OrderDto
                    {
                        ID = o.ID,
                        Title = o.Title,
                        Price = o.Price,
                        Field1 = o.Field1,
                        Field2 = o.Field1,
                        Field3 = o.Field3,
                        Field4 = o.Field4,
                        Field5 = o.Field5,
                        CreatedAt = o.CreatedAt,
                        Username = o.User != null ? o.User.Username : "Unknown" // Handle missing User gracefully
                    })
                    .ToListAsync();

                if (orders == null || !orders.Any())
                {
                    return NotFound("No orders found.");
                }

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while fetching orders: {ex.Message}");
            }
        }



    }
}