using BqsClinoTag.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace BqsClinoTag.Controllers
{
    public class ChatController : Controller
    {
        private readonly IHubContext<ChatHub> _chatHubContext;

        public ChatController(IHubContext<ChatHub> chatHubContext)
        {
            _chatHubContext = chatHubContext;
        }

        public async Task<IActionResult> ChatAsync([FromQuery(Name = "location")] string locationName)
        {
            if (!string.IsNullOrEmpty(locationName))
            {
                locationName = locationName.Trim('"'); // Remove quotes if present
            }

            ViewBag.RoomId = locationName;
            return View();
        }
    }
}
