﻿
using Microsoft.AspNetCore.Mvc;

using GP.APIs.DTOs;
using GP.APIs.Errors;


using GP.Core.IRepository;

using GP.Core.Entities;

using GP.Core.Specifications;
using AutoMapper;

namespace GP.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IRepository<Product> _Repo;
        private readonly IMapper _mapper;

        public ProductsController(IRepository<Product> Repo, IMapper mapper) {
            _Repo = Repo;
            _mapper = mapper;
        }



        //Get all products
        
        [HttpGet("GetAllProducts")]

        public async Task<ActionResult<IEnumerable<Product>>> GetProducts([FromQuery] ProductSpecParams Params)
        {
            var Spec = new ProductWithSpec(Params);
            var Products = await _Repo.GetAllWithSpecAsync(Spec);
            var MappedProducts = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(Products);

            return Ok(MappedProducts);
            
        }

        //get products by id
        [HttpGet("{id}")]

        public async Task<ActionResult<Product>> GetProductById(int id)
        {

            var Product =  _Repo.GetOne(null,p => p.Id == id,true);
            if (Product == null) return NotFound(new ApiResponse(404));
            var MappedProducts = _mapper.Map<Product, ProductToReturnDto>(Product);
            return Ok(MappedProducts);

          
        }


        //delete product
        [HttpDelete("{id}")]

        public IActionResult DeleteProductById(int id)
        {
            var product = _Repo.GetOne(null, p => p.Id == id, true);
            if (product == null)
            {
                return NotFound(new ApiResponse(404));
            }
            _Repo.Delete(product);
            _Repo.Commit();
            return Ok();
        }
        //Upsert
        //insert id for update
        [HttpPost("UpdateAndCreateProduct")]
        public IActionResult AddOrUpdateProduct([FromBody] Product product)
        {
            if (product == null)
            {
                return NotFound(new ApiResponse(404));
            }

            var existingProduct = _Repo.GetOne(null, p => p.Id == product.Id, true);
            //update product 
            if (existingProduct != null)
            {
                 _Repo.Edit(existingProduct);
                
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.StockQuantity = product.StockQuantity;
                existingProduct.ImageUrl = product.ImageUrl;
                existingProduct.Description = product.Description;
            }
            else
            {
                // Add new product
                _Repo.Create(product);
            }

            _Repo.Commit();
            return Ok();
        }


    }
}
