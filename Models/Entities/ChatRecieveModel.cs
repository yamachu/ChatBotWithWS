using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ChatBotWithWS.Models.Entities
{
    public class ChatRecieveModel
    {
        [JsonProperty("text")]
        public string Text;
        [JsonProperty("token")]
        public string Token;
    }
}