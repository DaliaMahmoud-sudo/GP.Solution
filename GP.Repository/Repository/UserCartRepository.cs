using AutoMapper;

using GP.Core.Entities;
using GP.Core.Entities.Identity;
using GP.Core.IRepository;
using GP.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GP.Service.Repository
{
    public class UserCartRepository : Repository<UserCart> , IUserCartRepository
    {
        private readonly StoreContext _dbContext;
        private readonly IRepository<UserCart> _cartRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IMapper _mapper;

        public UserCartRepository(StoreContext dbContext, IRepository<UserCart> cartRepository, IRepository<Product> productRepository, IMapper mapper) : base(dbContext)
        {
            _dbContext=dbContext;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public void AddProductToCart(string userId, int productId, int quantity)
        {
            throw new NotImplementedException();
        }


        //public void AddProductToCart(string userId, int productId, int quantity)
        //{
        //    // Check if product exists
        //    var product = _productRepository.GetOne(null, p => p.Id == productId, true);
        //    var MappedProduct = _mapper.Map<Product, ProductToReturnDto>(product);
        //    if (MappedProduct == null)
        //    {
        //        throw new KeyNotFoundException("Product not found.");
        //    }

        //    // Check if the user already has a cart
        //    var cart = _cartRepository.GetOne(null, c => c.UserId == userId, true);
        //    if (cart == null)
        //    {
        //        //Create a new cart for the user
        //        cart = new UserCart { UserId = userId, Items = new List<CartItems>() };
        //        _cartRepository.Create(cart);
        //    }

        //    // Check if the product already exists in the cart
        //    var cartItem = cart.Items.FirstOrDefault(ci => ci.productName == MappedProduct.Name);

        //    if (cartItem != null)
        //    {

        //        // Increase quantity if product is already in the cart
        //        cartItem.Quantity += quantity;
        //    }

        //    else
        //    {
        //        //  Add a new product to the cart
        //        cart.Items.Add(new CartItems
        //        {

        //            productName = MappedProduct.Name,
        //            ImageUrl = MappedProduct.ImageUrl,
        //            Price = MappedProduct.Price,
        //            Quantity = quantity
        //        });
        //    }


        //    _cartRepository.Commit();
        //}



    }






}

