using GP.Core.Entites.OrderAggregate;
using GP.Core.Entities.Identity;
using GP.Core.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Talabat.Core.Entites.Identity;

namespace GP.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository orderRepository;
        private readonly IOrderItemRepository orderItemRepository;
        private readonly UserManager<AppUser> userManager;

        public OrderController(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, UserManager<AppUser> userManager)
        {
            this.orderRepository=orderRepository;
            this.orderItemRepository=orderItemRepository;
            this.userManager=userManager;
        }

        [HttpGet("GetOrders")]
        public IActionResult GetOrders()
        {
            var orders = orderRepository.Get(includeProps: [e => e.Items , e => e.ShippingAddress , e => e.DeliveryMethod ]);
            return Ok(orders);
        }

        [HttpGet("GetOrderById")]
        public IActionResult GetOrderById(int orderId)
        {
            var order = orderRepository.GetOne(
                includeProps: [e => e.Items, 
                    e => e.ShippingAddress, 
                    e => e.DeliveryMethod],
                expression: e => e.Id == orderId);

            if (order != null)
            {
                return Ok(order);
            }

            return NotFound();

        }
        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized("User is not authenticated.");
            }

            var userName = User.Identity.Name;
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized($"User '{userName}' not found.");
            }

            order.BuyerEmail = "test@gmail.com";
            order.OrderDate = DateTimeOffset.UtcNow;
            order.ShippingAddress.FirstName = "test";
            order.ShippingAddress.LastName = "test";
            if (order.ShippingAddress == null ||
        string.IsNullOrWhiteSpace(order.ShippingAddress.City) ||
        string.IsNullOrWhiteSpace(order.ShippingAddress.Street) ||
        string.IsNullOrWhiteSpace(order.ShippingAddress.Country))
            {
                return BadRequest("Invalid shipping address.");
            }

            if (order.DeliveryMethod == null || order.Items == null || order.Items.Count == 0)
            {
                return BadRequest("Invalid order details.");
            }

            order.SubTotal = order.Items.Sum(item => item.Quantity * item.Price);
            
            orderRepository.Create(order);
            orderRepository.Commit();
            return Ok(order);
        }
    }
}
