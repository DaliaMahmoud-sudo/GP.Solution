using AutoMapper;
using GP.APIs.DTOs;
using GP.APIs.Errors;
using GP.Core.Entities;
using GP.Core.Entities.Identity;
using GP.Core.IRepository;
using GP.Repository.Data;
using GP.Repository.Repository;
using GP.Service.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Linq.Expressions;
using System.Security.Claims;

namespace GP.APIs.Controllers
{
    // var userId = "83ef60e1-87e9-4b90-bbbe-b697b3364371";




    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private readonly IUserCartRepository _userCartRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICartItemsRepository _cartItemsRepository;
        private readonly IPaymentRepository paymentRepository;

        public CartController(IUserCartRepository userCartRepository, IProductRepository productRepository, IMapper mapper, UserManager<AppUser> userManager, ICartItemsRepository cartItemsRepository
            ,IPaymentRepository paymentRepository)
        {
            _userCartRepository = userCartRepository;
            _productRepository = productRepository;
            _mapper = mapper;
            _userManager = userManager;
            _cartItemsRepository = cartItemsRepository;
            this.paymentRepository=paymentRepository;
        }
        [HttpPost("add")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> AddProductAndCreateCart(int productId, int quantity)
        {
            // Get the current user's email from the token
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                return Unauthorized(new ApiResponse(401));

            // Get user from the database
            var currentUser = await _userManager.FindByEmailAsync(email);
            if (currentUser == null)
                return Unauthorized(new ApiResponse(401));

            var userId = currentUser.Id;

            // Get the product from the repository
            var product = _productRepository.GetOne(null, p => p.Id == productId, true);
            if (product == null)
                return NotFound(new ApiResponse(404, "Product not found"));

            if (product.StockQuantity < quantity)
                return BadRequest(new ApiResponse(400, "There is not enough stock available"));

            // Deduct the stock
            product.StockQuantity -= quantity;

            // Map the product to DTO
            var mappedProduct = _mapper.Map<Product, ProductToReturnDto>(product);
            if (mappedProduct == null)
                return NotFound(new ApiResponse(404, "Mapping failed"));

            // Check if the user already has a cart
            var cart = _userCartRepository.GetOne(null, c => c.UserId == userId, true);
            if (cart == null)
            {
                cart = new UserCart
                {
                    UserId = userId,
                    Items = new List<CartItems>()
                };
                _userCartRepository.Create(cart);
            }

            // Check if product is already in the cart
            var cartItem = FindCartItemByProductName(cart, mappedProduct.Name);
            if (cartItem != null)
            {
                // Update quantity
                cartItem.Quantity += quantity;
            }
            else
            {
                // Add new item to the cart
                cart.Items.Add(new CartItems
                {
                    productName = mappedProduct.Name,
                    ImageUrl = mappedProduct.ImageUrl,
                    Price = mappedProduct.Price,
                    Quantity = quantity
                });
            }

            // Save changes
            _userCartRepository.Commit();

            return Ok(new ApiResponse(200, "Product added to cart successfully"));
        }


        //get cart by id
        [Authorize(Roles = "Client")]
        [HttpGet("GetUserCart")]

        public async Task<ActionResult<UserCart>> GetCart()
        {
            // Get the current user's email from the token
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                return Unauthorized(new ApiResponse(401));

            // Get user from the database
            var currentUser = await _userManager.FindByEmailAsync(email);
            if (currentUser == null)
                return Unauthorized(new ApiResponse(401));

            var id = currentUser.Id;
            if (string.IsNullOrEmpty(id))
                return Unauthorized(new ApiResponse(401));
            var cart = _userCartRepository.GetOne(includeProps: new Expression<Func<UserCart, object>>[]
    {
        cart => cart.Items // Include the UserCartItems
    }, c => c.UserId == id, true);
            if (cart == null) return NotFound(new ApiResponse(404));

            return Ok(cart);
        }
        [Authorize(Roles = "Client")]
        [HttpDelete("remove-product")]
        public async Task<IActionResult> RemoveProductFromCart(int itemId)
        {
            

            // Get the current user's email from the token
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                return Unauthorized(new ApiResponse(401));

            // Get user from the database
            var currentUser = await _userManager.FindByEmailAsync(email);
            if (currentUser == null)
                return Unauthorized(new ApiResponse(401));

            var userId = currentUser.Id;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new ApiResponse(401));



            // Fetch the user's cart
            var cart = _userCartRepository.GetOne(includeProps: new Expression<Func<UserCart, object>>[]
    {
        cart => cart.Items // Include the UserCartItems
    }, c => c.UserId == userId, true);

            if (cart == null)
            {
                return NotFound("Cart not found.");
            }


            // Find the product in the cart
            var cartItem = _cartItemsRepository.GetOne(
    null,
    ci => ci.Id == itemId && ci.UserCartId == cart.Id,
    true
);
            if (cartItem == null)
            {
                return NotFound("Product not found in cart.");
            }

            // Remove the product from the cart
            cart.Items.Remove(cartItem);
          var  Product = _productRepository.GetOne(null, nop => nop.Name == cartItem.productName ,true);
            if (Product == null) { return NotFound("not found product."); }
            Product.StockQuantity += cartItem.Quantity;

            // Save changes
            _userCartRepository.Edit(cart);
            _userCartRepository.Commit();
            _productRepository.Edit(Product);
            _productRepository.Commit();



            return Ok(new { message = "Product removed from cart." });
        }
        [Authorize(Roles = "Client")]
        [HttpGet("cart-total")]
        public async Task<IActionResult> GetCartTotal()
        {
            // Get the current user's email from the token
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                return Unauthorized(new ApiResponse(401));

            // Get user from the database
            var currentUser = await _userManager.FindByEmailAsync(email);
            if (currentUser == null)
                return Unauthorized(new ApiResponse(401));

            var userId = currentUser.Id;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new ApiResponse(401));


            var cart = _userCartRepository.GetOne(includeProps: new Expression<Func<UserCart, object>>[]
        {
                cart => cart.Items // Include the UserCartItems
            }, c => c.UserId == userId, true);
            if (cart == null) return NotFound("Cart not found.");

            var total = cart.Items.Sum(item => item.Price * item.Quantity);
            return Ok(new { totalPrice = total });
        }

        private CartItems FindCartItemByProductName(UserCart cart, string ProductName)
        {
            var cartItem = _cartItemsRepository.GetOne(
            null,
                ci => ci.productName == ProductName && ci.UserCartId == cart.Id,
             true
                    );

            return cartItem; // returns null if not found
        }

        [Authorize(Roles = "Client")]
        //[Authorize]
        [HttpPost("Pay")]
        public async Task<IActionResult> Pay()
        {
            // 1. Get current user
            // Get the current user's email from the token
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                return Unauthorized(new ApiResponse(401));

            // Get user from the database
            var currentUser = await _userManager.FindByEmailAsync(email);
            if (currentUser == null)
                return Unauthorized(new ApiResponse(401));

            var userId = currentUser.Id;
            var firstname = currentUser.FirstName;
            var lastname = currentUser.LastName;

            //if (string.IsNullOrEmpty(userId))
            //    return Unauthorized(new ApiResponse(401));

            //var user = await _userManager.FindByIdAsync(userId);
            //if (user == null)
            //    return Unauthorized(new ApiResponse(401));


            var cart = _userCartRepository.GetOne(
                includeProps: [c => c.Items],
                expression: c => c.UserId == userId,
                tracked: true
            );

            // 3. Validate cart
            if (cart == null || cart.Items == null || !cart.Items.Any())
                return BadRequest("Your cart is empty.");

            // 4. Calculate total
            var totalPrice = cart.Items.Sum(item => item.Price * item.Quantity);

            // 5. Create payment record
            var payment = new Payment
            {
                UserId = userId,
                Name = $"{firstname} {lastname}",
                Email = email,
                TotalPrice = totalPrice,
                PaymentDate = DateTimeOffset.UtcNow
            };
            paymentRepository.Create(payment);

            // 6. Prepare Stripe session
            var sessionOptions = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = cart.Items.Select(item => new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.productName,
                        },
                        UnitAmount = (long)(item.Price * 100),
                    },
                    Quantity = item.Quantity,
                }).ToList(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/checkout/success?payment_id={payment.paymentId}",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/checkout/cancel",
                CustomerEmail = email,
                Metadata = new Dictionary<string, string>
        {
            {"payment_id", payment.paymentId.ToString()},
            {"user_id", userId}
        }
            };

            // 7. Create Stripe session
            var sessionService = new SessionService();
            var session = sessionService.Create(sessionOptions);

           
            paymentRepository.Commit();

            cart.Items.Clear();
            _userCartRepository.Commit();

            // 10. Return Stripe URL
            return Ok(new { url = session.Url });
        }



    }
}


