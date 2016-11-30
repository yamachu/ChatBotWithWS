using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ChatBotWithWS.Models.Entities
{
    public class ChatRecieveModel
    {
        [JsonProperty("text", Required = Required.Always)]
        public string Text;
        [JsonProperty("token")]
        public string Token;
        [JsonProperty("csx")]
        public string CSX;
    }
}