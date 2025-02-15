using GP.APIs.DTOs;
using GP.Core.Entites.OrderAggregate;
using GP.Core.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace GP.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryMethodController : ControllerBase
    {
        private readonly IDeliveryMethodRepository deliveryMethodRepository;

        public DeliveryMethodController(IDeliveryMethodRepository deliveryMethodRepository)
        {
            this.deliveryMethodRepository=deliveryMethodRepository;
        }

        [HttpGet("GetDeliveryMethods")]
        public IActionResult GetDeliveryMethods()
        {
            var deliverymethods = deliveryMethodRepository.Get();
            return Ok(deliverymethods);
        }

        [HttpGet("GetDeliveryMethodbyId")]
        public IActionResult GetDeliveryMethodbyId(int Id)
        {
            var deliverymethod = deliveryMethodRepository.GetOne(expression: e => e.Id == Id);
            if (deliverymethod != null)
            { 
                return Ok(deliverymethod); 
            }
            return NotFound();
             
        }

        [HttpPost("CreateDeliveryMethod")]
        public IActionResult CreateDeliveryMethod(DeliveryMethodDto deliveryMethodDto)
        {
            if(ModelState.IsValid)
            {
                var deliverymethod = new DeliveryMethod
                {
                    ShortName = deliveryMethodDto.ShortName,
                    Description = deliveryMethodDto.Description,
                    DeliveryTime = deliveryMethodDto.DeliveryTime,
                    Cost = deliveryMethodDto.Cost
                };
                deliveryMethodRepository.Create(deliverymethod);
                deliveryMethodRepository.Commit();
                return Ok();
            }
    
            return BadRequest(deliveryMethodDto);
        }

        [HttpPut("UpdateDeliveryMethod")]
        public IActionResult UpdateDeliveryMethod(DeliveryMethodDto deliveryMethodDto)
        {
            if (ModelState.IsValid)
            {
                var deliverymethod = new DeliveryMethod
                {
                    Id = deliveryMethodDto.Id,
                    ShortName = deliveryMethodDto.ShortName,
                    Description = deliveryMethodDto.Description,
                    DeliveryTime = deliveryMethodDto.DeliveryTime,
                    Cost = deliveryMethodDto.Cost
                };
                deliveryMethodRepository.Edit(deliverymethod);
                deliveryMethodRepository.Commit();
                return Ok();
            }

            return BadRequest();
        }

        [HttpDelete("DeleteDeliveryMethod")]
        public IActionResult DeleteDeliveryMethod(int Id)
        {
            var deliverymethod = deliveryMethodRepository.GetOne(expression: e => e.Id == Id);
            if (deliverymethod != null)
            {
                deliveryMethodRepository.Delete(deliverymethod);
                deliveryMethodRepository.Commit();
                return Ok();

            }
            return NotFound();
        }

    }
}
