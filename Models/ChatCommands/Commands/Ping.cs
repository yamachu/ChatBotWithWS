using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using ChatBotWithWS.Models;
using ChatBotWithWS.Models.Entities;

namespace ChatBotWithWS.Models.ChatCommands.Commands
{
    public class Ping : IChatCommand
    {
        public ChatTransferModel Run(CommandModel model)
        {
            var trans = new Models.Entities.ChatTransferModel();
            trans.MessageType = ChatMessageType.Bot;
            trans.Success = true;
            trans.Text = "pong";

            return trans;
        }

        public string Usage()
        {
            return "Return \"pong\"";
        }
    }




}