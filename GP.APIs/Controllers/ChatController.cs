using GP.APIs.DTOs;
using GP.APIs.Errors;
using GP.Core.Entities.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GP.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly UserManager<AppUser> userManager;

        public ChatController(IHttpClientFactory httpClientFactory , UserManager<AppUser> userManager)
        {
            _httpClient = httpClientFactory.CreateClient();
            this.userManager=userManager;
        }


        [HttpPost("ask-bot")]
        public async Task<IActionResult> AskBot([FromBody] ChatRequestDTO request)
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

            //string userId = HttpContext.Connection.RemoteIpAddress?.ToString(); // Or get from request

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID could not be determined.");
            }

            // Load existing history or create new
            if (!ChatMemory.UserHistories.TryGetValue(userId, out var history))
            {
                history = new List<ConversationEntry>();
                ChatMemory.UserHistories[userId] = history;
            }

            // Append new user input
            history.Add(new ConversationEntry { user = request.user_input });

            // Prepare request to Python API
            var pythonRequest = new ChatRequestDTO
            {
                user_input = request.user_input,
                conversation_history = history
            };

            using var client = new HttpClient();
            var pythonApiUrl = "https://2209-34-48-159-64.ngrok-free.app/chat";

            var response = await client.PostAsJsonAsync(pythonApiUrl, pythonRequest);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, $"Python API error:\n{error}");
            }

            var result = await response.Content.ReadFromJsonAsync<ChatResponseDTO>();

            // Add bot response to history
            history[history.Count - 1].bot = result.bot_response;

            return Ok(result);
        }



    }
}
