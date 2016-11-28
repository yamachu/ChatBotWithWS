using System;

namespace ChatBotWithWS.Models.Entities
{
    public enum ChatMessageType
    {
        Message,
        Bot,
    }

    // http://qiita.com/hugo-sb/items/38fe86a09e8e0865d471
    static class ChatMessageTypeExt{
        public static string DisplayName(this ChatMessageType messageType)
        {
            string[] names = {"message", "bot"};
            return names[(int)messageType];
        }
    }
}