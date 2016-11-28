using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace ChatBotWithWS.Models
{
    public class ChatUser
    {
        public WebSocket Socket{get; private set;}

        // Will support user-name and chat-room
        public string Name{get;set;}

        public string Room{get;set;}

        public ChatUser(WebSocket socket)
        {
            Socket = socket;
        }
    }
}