using AutoMapper;
using GP.APIs.DTOs;
using GP.APIs.Errors;
using GP.Core.Entities;
using GP.Core.Entities.Identity;
using GP.Core.IRepository;
using GP.Repository.Data;
using GP.Service.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
 

        public CartController(IUserCartRepository userCartRepository, IProductRepository productRepository, IMapper mapper, UserManager<AppUser> userManager, ICartItemsRepository cartItemsRepository)
        {
            _userCartRepository = userCartRepository;
            _productRepository = productRepository;
            _mapper = mapper;
            _userManager = userManager;
            _cartItemsRepository = cartItemsRepository;
        }
        [HttpPost("add")]
        public IActionResult AddProductAndCreateCart(int productId, int quantity)
        {

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
               return Unauthorized(new ApiResponse(401));
            
            var product = _productRepository.GetOne(null, p => p.Id == productId, true);
            if (product.StockQuantity < quantity)
            {
                return BadRequest(new ApiResponse(400, "there is no enough product sorry"));
            }
            else
            {
                product.StockQuantity-=quantity;
            }
            var MappedProduct = _mapper.Map<Product, ProductToReturnDto>(product);
            if (MappedProduct == null)
            {
                return NotFound(new ApiResponse(404));
            }

            // Check if the user already has a cart
            var cart = _userCartRepository.GetOne(null, c => c.UserId == userId, true);
            if (cart == null)
            {
                //Create a new cart for the user
                cart = new UserCart { UserId = userId, Items = new List<CartItems>() };
                _userCartRepository.Create(cart);
            }


            // Check if the product already exists in the cart
            var cartItem = FindCartItemByProductName(cart, MappedProduct.Name);

            if (cartItem != null)
            {

                // Increase quantity if product is already in the cart
                cartItem.Quantity += quantity;
            }

            else
            {
                //  Add a new product to the cart
                cart.Items.Add(new CartItems
                {

                    productName = MappedProduct.Name,
                    ImageUrl = MappedProduct.ImageUrl,
                    Price = MappedProduct.Price,
                    Quantity = quantity
                });
            }


            _userCartRepository.Commit();
            return Ok();
        }
     
        
        //get cart by id
        [HttpGet("GetUserCart")]

        public async Task<ActionResult<UserCart>> GetCart()
        {
            var id=_userManager.GetUserId(User);
            if (string.IsNullOrEmpty(id))
                return Unauthorized(new ApiResponse(401));
            var cart = _userCartRepository.GetOne(includeProps: new Expression<Func<UserCart, object>>[]
    {
        cart => cart.Items // Include the UserCartItems
    }, c => c.UserId == id, true);
            if (cart == null) return NotFound(new ApiResponse(404));

            return Ok(cart);
        }
        [HttpDelete("remove-product")]
        public async Task<IActionResult> RemoveProductFromCart(int itemId)
        {
            // Get user ID from cookies
            
            var userId = _userManager.GetUserId(User);
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

            // Save changes
            _userCartRepository.Edit(cart);
             _userCartRepository.Commit();



            return Ok(new { message = "Product removed from cart." });
        }
        [HttpGet("cart-total")]
        public async Task<IActionResult> GetCartTotal()
        {
            var userId = _userManager.GetUserId(User);
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


    }

}


