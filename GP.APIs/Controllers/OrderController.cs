using GP.Core.Entites.OrderAggregate;
using GP.Core.Entities.Identity;
using GP.Core.IRepository;
using GP.Service.Repository;
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
        private readonly IDeliveryMethodRepository deliveryMethodRepository;
        private readonly UserManager<AppUser> userManager;
        private readonly IProductRepository productRepository;

        public OrderController(IOrderRepository orderRepository, 
            IOrderItemRepository orderItemRepository, 
            IDeliveryMethodRepository deliveryMethodRepository,
            UserManager<AppUser> userManager,
            IProductRepository productRepository)
        {
            this.orderRepository=orderRepository;
            this.orderItemRepository=orderItemRepository;
            this.deliveryMethodRepository=deliveryMethodRepository;
            this.userManager=userManager;
            this.productRepository=productRepository;
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
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized("User is not authenticated.");
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            // 🛑 Load existing delivery method instead of creating a new one
            var existingDeliveryMethod =  deliveryMethodRepository.GetOne(expression: e => e.Id == order.DeliveryMethod.Id);
            if (existingDeliveryMethod == null)
            {
                return BadRequest("Invalid delivery method.");
            }
            order.DeliveryMethod = existingDeliveryMethod; // Use existing one

            // 🛑 Ensure shipping address exists
            if (order.ShippingAddress == null)
            {
                order.ShippingAddress = new ShippingAddress();
            }

            order.BuyerEmail = user.Email;
            order.OrderDate = DateTimeOffset.UtcNow;
            order.ShippingAddress.FirstName = user.FirstName;
            order.ShippingAddress.LastName = user.LastName;

            // 🛑 Ensure that the same product is not added twice in the same order
            var validatedItems = new List<OrderItem>();

            foreach (var item in order.Items)
            {
                // 🛑 Fetch the existing OrderItem from DB (assuming it exists)
                var existingOrderItem = orderItemRepository.GetOne(expression:e => e.Id == item.Id);

                if (existingOrderItem == null)
                {
                    return BadRequest($"Order item with ID {item.Id} not found.");
                }

                // 🛑 Ensure the product is properly loaded from DB
                var existingProduct = productRepository.GetOne(expression:e => e.Id == existingOrderItem.Product.ProductId);

                if (existingProduct == null)
                {
                    return BadRequest($"Product with ID {existingOrderItem.Product.ProductId} not found.");
                }

                // ✅ Assign fetched ProductItemOrdered
                existingOrderItem.Product = new ProductItemOrdered
                {
                    ProductId = existingProduct.Id,
                    ProductName = existingProduct.Name,
                    ImageUrl = existingProduct.ImageUrl
                };

                // ✅ Ensure the quantity and price are updated
                existingOrderItem.Quantity = item.Quantity;
                existingOrderItem.Price = existingProduct.Price;

                // ✅ Add the validated order item
                validatedItems.Add(existingOrderItem);
            }

            // ✅ Assign the validated items back to the order
            order.Items = validatedItems;


            order.SubTotal = order.Items.Sum(item => item.Quantity * item.Price) + order.GetTotal();

            orderRepository.Create(order);
            orderRepository.Commit();

            return Ok(order);
        }



    

    }
}
