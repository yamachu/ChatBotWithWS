using System;
using System.ComponentModel;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ChatBotWithWS.Models.Entities
{
    public enum ChatTransferModelType {
        FORMAT_ERROR,
        SECRET_MESSAGE,
        BROADCAST,
    }
    public class ChatTransferModel
    {
        [JsonProperty("text", Required = Required.Always)]
        public string Text;

        [JsonIgnore]
        public ChatMessageType MessageType;

        [JsonProperty("type", Required = Required.Always)]
        public string Type => MessageType.DisplayName();
        [JsonProperty("success", Required = Required.Always)]
        public bool Success;

        #region WillSupport
        [JsonProperty("from")]
        [DefaultValue("anonymous")]
        [JsonIgnore]
        public string From {get;set;} = "anonymous";

        [JsonIgnore]
        public int? Target {get;set;} = null;
        [JsonIgnore]
        public WebSocketMessageType SocketMessageType{get;private set;} = WebSocketMessageType.Text;
        #endregion

        public static ChatTransferModel CreateModel(ChatTransferModelType type, int? target = null)
        {
            var model = new ChatTransferModel();
            switch(type)
            {
                case ChatTransferModelType.FORMAT_ERROR:
                model.Text = "Unexpected format, require json => {text:\"FooBar\"}";
                model.Target = target.Value;
                model.Success = false;
                model.MessageType = ChatMessageType.Bot;
                break;
                case ChatTransferModelType.SECRET_MESSAGE:
                model.Target = target.Value;
                model.MessageType = ChatMessageType.Bot;
                break;
                // Will not call
                case ChatTransferModelType.BROADCAST:
                model.Text = "Internal Error";
                model.Success = false;
                model.MessageType = ChatMessageType.Bot;
                break;
            }
            return model;
        }

        public static ChatTransferModel FromRecieveModel(ChatRecieveModel model)
        {
            return new ChatTransferModel
            {
                Text = model.Text ?? "",
                MessageType = ChatMessageType.Message,
                Success = true
            };
        }
    }
}