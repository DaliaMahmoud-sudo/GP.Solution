using GP.APIs.DTOs;
using GP.APIs.Errors;
using GP.Core.Entities.Identity;
using GP.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
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
        private readonly ITokenService _tokenService;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager, ITokenService tokenService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            _tokenService = tokenService;
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
                        return Ok(new AppUserDto()
                        {

                            FirstName = userDb.FirstName,
                            LastName = userDb.LastName,
                            Email = userDb.Email,
                            Token = await _tokenService.CreateTokenAsync(userDb, userManager)

                        });
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
        [Authorize]
        [HttpGet("GetCurrentUser")]
        public async Task<ActionResult<AppUserDto>> GetCurrentUser()
        {
            var Email = User.FindFirstValue(ClaimTypes.Email);
            var user = await userManager.FindByEmailAsync(Email);
            var ReturnedObject = new AppUserDto()
            {
                Id=user.Id,
                FirstName = user.FirstName,
                Email = user.Email,
                Token = await _tokenService.CreateTokenAsync(user, userManager)
            };
            return Ok(ReturnedObject);
        }
        [HttpGet("emailExists")]
        public async Task<ActionResult<bool>> CheckEmailExists(string Email)
        {
            return await userManager.FindByEmailAsync(Email) is not null;

        }
    }
}
