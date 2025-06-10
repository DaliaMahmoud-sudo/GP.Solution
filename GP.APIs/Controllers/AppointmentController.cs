using GP.APIs.DTOs;
using GP.APIs.Errors;
using GP.Core.Entities;
using GP.Core.Entities.Identity;
using GP.Core.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
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
        [HttpGet("GetAppointmentsForDoctor")]
        public async Task<IActionResult> GetAppointmentsForDoctor()
        {
            if (!User.IsInRole("Doctor"))
            {
                return Forbid("Access denied: Only doctors can view these appointments.");
            }

            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                return Unauthorized(new ApiResponse(401));

            var currentUser = await userManager.FindByEmailAsync(email);
            if (currentUser == null)
                return Unauthorized(new ApiResponse(401));

            var doctorId = currentUser.Id;

            // Step 1: Get all appointments for this doctor
            var appointments = appointmentRepository
                .Get(expression: a => a.DoctorId == doctorId)
                .ToList();

            if (!appointments.Any())
            {
                return NotFound("No appointments found for this doctor.");
            }

            // Step 2: Fetch related users (clients)
            var userIds = appointments.Select(a => a.UserId).Distinct();

            var users = userManager.Users
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new { u.Id, FullName = u.FirstName + " " + u.LastName })
                .ToList();

            // Step 3: Build result
            var result = appointments.Select(a => new
            {
                a.AppointmentId,
                a.AppointmentDate,
                a.Note,
                PatientName = users.FirstOrDefault(u => u.Id == a.UserId)?.FullName,
                DoctorName = currentUser.FirstName + " " + currentUser.LastName
            });

            return Ok(result);
        }



        [Authorize(Roles = "Client")]
        [HttpGet("GetAppointmentsForUser")]
        public async Task<IActionResult> GetAppointmentsForUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                return Unauthorized(new ApiResponse(401));

            var currentUser = await userManager.FindByEmailAsync(email);
            if (currentUser == null)
                return Unauthorized(new ApiResponse(401));

            var userId = currentUser.Id;

            // Step 1: Get appointments for current user
            var appointments = appointmentRepository
                .Get(expression: a => a.UserId == userId)
                .ToList();

            if (!appointments.Any())
                return NotFound("No appointments found.");

            // Step 2: Manually fetch doctor and user names
            var doctorIds = appointments.Select(a => a.DoctorId).Distinct();
            var userIds = appointments.Select(a => a.UserId).Distinct();

            var doctors = userManager.Users
                .Where(u => doctorIds.Contains(u.Id))
                .Select(u => new { u.Id, FullName = u.FirstName + " " + u.LastName })
                .ToList();

            var users = userManager.Users
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new { u.Id, FullName = u.FirstName + " " + u.LastName })
                .ToList();

            // Step 3: Project final result
            var result = appointments.Select(a => new
            {
                a.AppointmentId,
                a.AppointmentDate,
                a.Note,
                DoctorName = doctors.FirstOrDefault(d => d.Id == a.DoctorId)?.FullName,
                UserName = users.FirstOrDefault(u => u.Id == a.UserId)?.FullName
            });

            return Ok(result);
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
