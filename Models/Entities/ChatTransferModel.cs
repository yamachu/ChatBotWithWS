using System;
using System.ComponentModel;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ChatBotWithWS.Models.Entities
{

    public class ChatTransferModel
    {
        [JsonProperty("text")]
        public string Text;

        [JsonIgnore]
        public ChatMessageType MessageType;

        [JsonProperty("type")]
        public string Type => MessageType.DisplayName();
        [JsonProperty("success")]
        public bool Success;

        [JsonProperty("from")]
        [DefaultValue("anonymous")]
        public string From {get;set;} = "anonymous";
    }
}