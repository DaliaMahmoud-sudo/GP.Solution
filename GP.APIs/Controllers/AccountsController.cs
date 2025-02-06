using GP.APIs.Errors;
using GP.Core.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GP.APIs.DTOs;
using GP.APIs.Controllers;



namespace Talabat.APIs.Controllers
{
    public class AccountsController : APIBaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        

        public AccountsController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager

            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
           
        }
        //Register
        [HttpPost("Register")]
        public async Task<ActionResult<AppUserDto>> Register(RegisterDto model)
        {




            var User = new AppUser()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Email.Split('@')[0],
                PhoneNumber = model.PhoneNumber
            };
            var Result = await _userManager.CreateAsync(User, model.Password);
            if (!Result.Succeeded) return BadRequest(new ApiResponse(400)); 
            var ReturnedUser = new AppUserDto()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
               
            };
            return Ok(ReturnedUser);

        }



        //Login
        [HttpPost("Login")]
       public async Task<ActionResult<AppUserDto>> Login(LoginDto model)
       {
           var AppUser = await _userManager.FindByEmailAsync(model.Email);
           if (AppUser is null) return Unauthorized(new ApiResponse(401));
           var Result = await _signInManager.CheckPasswordSignInAsync(AppUser,
                                                                  model.Password,
                                                                   false);
           if (!Result.Succeeded) return Unauthorized(new ApiResponse(401));
            return Ok(new AppUserDto()
           {

                FirstName = AppUser.FirstName,
                LastName = AppUser.LastName,
                Email = AppUser.Email,
              

            });
        }
       

    }
} 
