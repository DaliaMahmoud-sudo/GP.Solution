using GP.APIs.DTOs;
using GP.APIs.Errors;
using GP.Core.Entities;
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
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentRepository appointmentRepository;
        private readonly UserManager<AppUser> userManager;

        public AppointmentController(IAppointmentRepository appointmentRepository, UserManager<AppUser> userManager)
        {
            this.appointmentRepository=appointmentRepository;
            this.userManager=userManager;
        }
        [Authorize(Roles = "Doctor")]
        [HttpGet(" GetAppointmentsForDoctor")]
        public async Task<IActionResult> GetAppointmentsForDoctor()
        {
            if (!User.IsInRole("Doctor"))
            {
                return Forbid("Access denied: Only doctors can view these appointments.");
            }

            // Get the current user's email from the token
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                return Unauthorized(new ApiResponse(401));

            // Get user from the database
            var currentUser = await userManager.FindByEmailAsync(email);
            if (currentUser == null)
                return Unauthorized(new ApiResponse(401));

            var doctorId = currentUser.Id;
            
            var appointments = appointmentRepository.Get(expression: e => e.DoctorId == doctorId);

            if (!appointments.Any())
            {
                return NotFound("No appointments found for this doctor.");
            }
            return Ok(appointments);
        }

        [Authorize(Roles ="Client")] // Ensures only authenticated users can access
        [HttpGet("GetAppointmentsForUser")]
        public async Task<IActionResult> GetAppointmentsForUser()
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


            var appointments = appointmentRepository.Get(expression: e => e.UserId == userId);

            if (!appointments.Any())
            {
                return NotFound("No appointments found.");
            }
            return Ok(appointments);
        }

        [Authorize(Roles = "Client")]
        [HttpPost("CreateAppointments")]
        public async Task<IActionResult> CreateAppointments([FromBody] AppointmentRequestDto appointmentRequestDto)
        {
            if (appointmentRequestDto == null || string.IsNullOrEmpty(appointmentRequestDto.DoctorId))
            {
                return BadRequest("DoctorId is required.");
            }

            // Get the current user's email from the token
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                return Unauthorized(new ApiResponse(401));

            // Get user from the database
            var currentUser = await userManager.FindByEmailAsync(email);
            if (currentUser == null)
                return Unauthorized(new ApiResponse(401));

            var userId = currentUser.Id;

            // Check if the doctor already has an appointment at the same time
            bool isDoctorBusy = appointmentRepository
                .Get(expression: a => a.DoctorId == appointmentRequestDto.DoctorId && a.AppointmentDate == appointmentRequestDto.AppointmentDate)
                .Any();

            if (isDoctorBusy)
            {
                return Conflict("The doctor is already booked at this time. Please choose another time.");
            }

            var appointment = new Appointment
            {
                UserId = userId,
                DoctorId = appointmentRequestDto.DoctorId,
                AppointmentDate = appointmentRequestDto.AppointmentDate,
                AppointmentStatus = Status.Pending,
                Note = appointmentRequestDto.Notes,
                CreatedAt = DateTimeOffset.UtcNow
            };

            appointmentRepository.Create(appointment);
            appointmentRepository.Commit();

            return Ok(new { Message = "Appointment created successfully", AppointmentId = appointment.AppointmentId });
        }

        [HttpDelete("DeleteAppointment")]
        public IActionResult DeleteAppointment (int appointmentId)
        {
            var appointment = appointmentRepository.GetOne(expression: e => e.AppointmentId == appointmentId);
            if (appointment != null)
            {
                appointmentRepository.Delete(appointment);
                appointmentRepository.Commit();
                return Ok("Appointemnt Deleted successfully...");
            }
            return NotFound();
        }

    }
}
