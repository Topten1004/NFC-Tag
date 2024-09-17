using BqsClinoTag.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.IdentityModel.Tokens.Jwt;

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
            int flag = 1;
            if (!string.IsNullOrEmpty(locationName))
            {
                if(locationName.Contains('"'))
                {
                    locationName = locationName.Trim('"'); // Remove quotes if present
                }
            }

            if(locationName == null)
            {
                locationName = "Managers";
                flag = 2;
            }


            ViewBag.RoomId = locationName;
            ViewBag.IsLoggin = flag;
            return View();
        }
    }
}
