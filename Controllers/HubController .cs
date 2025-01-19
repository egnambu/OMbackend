using Microsoft.AspNetCore.Mvc;

namespace OMbackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HubController : ControllerBase
    {
        [HttpGet("hub-url")]
        public IActionResult GetHubUrl()
        {
            // Return the SignalR hub URL
            return Ok(new { hubUrl = "https://localhost:8237/hub" });
        }
    }
}