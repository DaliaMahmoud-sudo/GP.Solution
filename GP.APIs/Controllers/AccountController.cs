﻿using GP.APIs.DTOs;
using GP.APIs.Errors;
using GP.Core.Entities.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Utility;

namespace GP.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(AppUserDto userDTO)
        {
            if (CheckEmailExists(userDTO.Email).Result.Value)
            {
                return BadRequest(new ApiResponse(400, "Email already exists"));
            }

            if (roleManager.Roles.IsNullOrEmpty())
            {
                await roleManager.CreateAsync(new(SD.AdminRole));
                await roleManager.CreateAsync(new(SD.ClientRole));
                await roleManager.CreateAsync(new(SD.DoctorRole));
            }
            if (ModelState.IsValid)
            {
                AppUser applicationUser = new AppUser()
                {
                    FirstName = userDTO.FirstName,
                    LastName = userDTO.LastName,
                    Email = userDTO.Email,
                    UserName = userDTO.Email.Split('@')[0],
                    PhoneNumber = userDTO.PhoneNumber
                };
                var result = await userManager.CreateAsync(applicationUser, userDTO.Password);
                if (result.Succeeded)
                {
                    // Ok
                    // Assign role to user
                    await userManager.AddToRoleAsync(applicationUser, SD.ClientRole);
                    await signInManager.SignInAsync(applicationUser, false);

                    return Ok();
                }

                return BadRequest(result.Errors);
            }

            return BadRequest(userDTO);


        }

        [HttpPost("RegisterDoctor")]
        public async Task<IActionResult> RegisterDoctor(RegisterDoctorDto userDTO)
        {
            if (CheckEmailExists(userDTO.Email).Result.Value)
            {
                return BadRequest(new ApiResponse(400, "Email already exists"));
            }

            if (roleManager.Roles.IsNullOrEmpty())
            {
                await roleManager.CreateAsync(new(SD.AdminRole));
                await roleManager.CreateAsync(new(SD.ClientRole));
                await roleManager.CreateAsync(new(SD.DoctorRole));
            }
            if (ModelState.IsValid)
            {
                Doctor applicationUser = new Doctor()
                {
                    FirstName = userDTO.FirstName,
                    LastName = userDTO.LastName,
                    Email = userDTO.Email,
                    UserName = userDTO.Email.Split('@')[0],
                    Specializtion = userDTO.Specializtion,
                    Bio = userDTO.Bio ?? "",
                    PhoneNumber = userDTO.PhoneNumber
                };
                var result = await userManager.CreateAsync(applicationUser, userDTO.Password);
                if (result.Succeeded)
                {
                    // Ok
                    // Assign role to user
                    await userManager.AddToRoleAsync(applicationUser, SD.DoctorRole);
                    await signInManager.SignInAsync(applicationUser, false);

                    return Ok();
                }

                return BadRequest(result.Errors);
            }

            return BadRequest(userDTO);


        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto userDTO)
        {
            if (ModelState.IsValid)
            {
                var userDb = await userManager.FindByEmailAsync(userDTO.Email);

                if (userDb != null)
                {
                    var finalResult = await userManager.CheckPasswordAsync(userDb, userDTO.Password);

                    if (finalResult)
                    {
                        // Login ==> Create ID (cookies)
                        await signInManager.SignInAsync(userDb, userDTO.RememberMe);
                        return Ok();
                    }
                    else
                        // Error in password
                        ModelState.AddModelError("Password", "invalid passwrod");
                }
                else
                    // Error in userName
                    ModelState.AddModelError("Email", "invalid Email");

            }
            return NotFound();
        }
        [HttpDelete("Logout")]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return Ok();
        }
        [HttpGet("emailExists")]
        public async Task<ActionResult<bool>> CheckEmailExists(string Email)
        {
            return await userManager.FindByEmailAsync(Email) is not null;

        }
    }
}
