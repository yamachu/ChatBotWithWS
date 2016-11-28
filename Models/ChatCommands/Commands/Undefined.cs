using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using ChatBotWithWS.Models;
using ChatBotWithWS.Models.Entities;

namespace ChatBotWithWS.Models.ChatCommands.Commands
{
    public class Undefined : IChatCommand
    {
        public ChatTransferModel Run(CommandModel model)
        {
            var trans = new Models.Entities.ChatTransferModel();
            trans.MessageType = ChatMessageType.Bot;
            trans.Success = false;
            trans.Text = Usage();

            return trans;
        }

        public string Usage()
        {
            return "Undefined command, try => bot help";
        }
    }




}