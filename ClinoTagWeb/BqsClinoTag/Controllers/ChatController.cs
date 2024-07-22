using Microsoft.AspNetCore.Mvc;

namespace BqsClinoTag.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Chat([FromQuery]string RoomId)
        {
            return View();
        }
    }
}
