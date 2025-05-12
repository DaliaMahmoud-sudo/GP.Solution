using GP.APIs.DTOs;
using GP.APIs.Errors;
using GP.Core.Entities;
using GP.Core.Entities.Identity;
using GP.Core.IRepository;
using GP.Repository.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GP.APIs.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly StoreContext _dbContext;

        public NotificationsController(INotificationRepository notificationRepository, UserManager<AppUser> userManager, StoreContext dbContext)
        {
            _notificationRepository = notificationRepository;
            _userManager = userManager;
            _dbContext = dbContext;
        }

        // GET: api/notifications
        [HttpGet]
        public async Task<IActionResult> GetUserNotifications()
        {
            //var userId = _userManager.GetUserId(User);

            ///Fetching User Data
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                return Unauthorized(new ApiResponse(401));

            // Get user from the database
            var currentUser = await _userManager.FindByEmailAsync(email);
            if (currentUser == null)
                return Unauthorized(new ApiResponse(401));

            var userId = currentUser.Id;
            ///

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated.");

            var notifications = await _notificationRepository.GetUserNotificationsAsync(userId);
            if (notifications == null || !notifications.Any())
                return NotFound("No notifications found for this user.");

            return Ok(notifications);
        }
        [HttpPut("MarkAsRead/{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _dbContext.notifications.FindAsync(id);

            if (notification == null)
                return NotFound(new { Message = "Notification not found." });

            notification.Status = Notification.MessageStatus.Read;
            await _dbContext.SaveChangesAsync();

            return Ok(new { Message = "Notification marked as read successfully." });
        }
       



    }
}
