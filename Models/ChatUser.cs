using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace ChatBotWithWS.Models
{
    public class ChatUser
    {
        // Will support user-name and chat-room
        public string Name{get;set;}

        public string Room{get;set;}

        public int UserHash{get;private set;}

        public ChatUser(int hash)
        {
            UserHash = hash;
        }
    }
}