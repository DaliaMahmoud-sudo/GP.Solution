using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace GP.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainController : ControllerBase
    {
        private readonly HttpClient _client = new();
        private readonly string _baseUrl = "http://127.0.0.1:5000";

        [HttpGet("start")]
        public async Task<IActionResult> Start() =>
            await ProxyToPython("/start");

        [HttpGet("stop")]
        public async Task<IActionResult> Stop() =>
            await ProxyToPython("/stop");

        [HttpGet("data")]
        public async Task<IActionResult> GetData()
        {
            var res = await _client.GetAsync($"{_baseUrl}/data");
            var json = await res.Content.ReadAsStringAsync();
            return Content(json, "application/json");
        }

        [HttpGet("set_exercise/{exercise}")]
        public async Task<IActionResult> SetExercise(string exercise) =>
            await ProxyToPython($"/set_exercise/{exercise}");

        [HttpGet("video")]
        public IActionResult GetVideoUrl()
        {
            return Ok($"{_baseUrl}/video_feed");
        }

        private async Task<IActionResult> ProxyToPython(string path)
        {
            var res = await _client.GetAsync($"{_baseUrl}{path}");
            var content = await res.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }
    }
}
