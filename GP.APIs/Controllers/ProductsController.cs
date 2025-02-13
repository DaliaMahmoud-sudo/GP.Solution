using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using GP.APIs.DTOs;
using GP.APIs.Errors;

using GP.Core.Entites;
using GP.Core.IRepository;

using GP.Core.Entities;
using Microsoft.AspNetCore.Hosting;
using GP.Service.Repository;

namespace GP.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IRepository<Product> _Repo;
       

        public ProductsController(IRepository<Product> Repo) {
            _Repo = Repo;
           
        }



        //Get all products
        
        [HttpGet]

        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var Products =  _Repo.Get();
            return Ok(Products);
        }

        //get products by id
        [HttpGet("{id}")]

        public async Task<ActionResult<Product>> GetProductById(int id)
        {

            var Product =  _Repo.GetOne(null,p => p.Id == id,true);
            if (Product == null) return NotFound(new ApiResponse(404));

            return Ok(Product);
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
        [HttpPost]
        public IActionResult AddOrUpdateProduct([FromBody] Product product)
        {
            if (product == null)
            {
                return NotFound(new ApiResponse(404));
            }

            var existingProduct = _Repo.GetOne(null, p => p.Id == product.Id, true);

            if (existingProduct != null)
            {
                
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
