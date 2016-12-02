using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ChatBotWithWS.Services;

namespace ChatBotWithWS.Controllers
{
    public class ChatController : Controller
    {
        private readonly IChatService _chatService;
        private readonly string DefaultRoom;

        public ChatController(IChatService chatService) 
        {
            _chatService = chatService;
            DefaultRoom = "Global";
        }
        public async Task<IActionResult> Index()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest) {
                await _chatService.HandleConnection(HttpContext);
                return Accepted();
            } else {
                ViewData["Room"] = DefaultRoom;
                return View();
            }
        }

        public async Task<IActionResult> Room(string id = "Global")
        {
            if (HttpContext.WebSockets.IsWebSocketRequest) {
                await _chatService.HandleConnection(HttpContext, id);
                return Accepted();
            } else {
                ViewData["Room"] = id;
                return View("Index");
            }
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
