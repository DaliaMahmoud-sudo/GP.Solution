﻿using GP.APIs.Errors;
using GP.Core.Entities.Identity;
using GP.Core.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GP.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentRepository paymentRepository;
        private readonly UserManager<AppUser> userManager;

        public PaymentController(IPaymentRepository paymentRepository, UserManager<AppUser> userManager)
        {
            this.paymentRepository=paymentRepository;
            this.userManager=userManager;
        }

        [HttpGet("GetAllPayments")]
        public IActionResult GetAllPayments()
        {
            var payments = paymentRepository.Get();

            if (!payments.Any())
            {
                return NotFound("No payments found.");
            }
            return Ok(payments);
        }

        [Authorize(Roles = "Client")]
        [HttpGet("GetPaymentsForUser")]
        public async Task<IActionResult> GetPaymentsForUser()
        {
            // Get the current user's email from the token
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                return Unauthorized(new ApiResponse(401));

            // Get user from the database
            var currentUser = await userManager.FindByEmailAsync(email);
            if (currentUser == null)
                return Unauthorized(new ApiResponse(401));

            var userId = currentUser.Id;

            /* var userId = userManager.GetUserId(User)*/
            ;
            var payments = paymentRepository.Get(expression: e => e.UserId == userId);

            if (!payments.Any())
            {
                return NotFound("No payments found.");
            }
            return Ok(payments);
        }

        [HttpDelete("DeletePayment")]
        public IActionResult DeletePayment(int paymentId)
        {
            var payment = paymentRepository.GetOne(expression: e => e.paymentId == paymentId);
            if (payment == null)
            {
                return NotFound("Payment not found.");
            }

            paymentRepository.Delete(payment);
            paymentRepository.Commit();

            return Ok("Payment deleted successfully.");
        }



    }
}
