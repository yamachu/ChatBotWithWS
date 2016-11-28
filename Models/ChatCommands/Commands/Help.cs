using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using ChatBotWithWS.Models;
using ChatBotWithWS.Models.Entities;

namespace ChatBotWithWS.Models.ChatCommands.Commands
{
    public class Help : IChatCommand
    {
        public ChatTransferModel Run(CommandModel model)
        {
            var trans = new Models.Entities.ChatTransferModel();
            trans.MessageType = ChatMessageType.Bot;
            trans.Success = true;
            trans.Text = CollectAllUsage();

            return trans;
        }

        public string Usage()
        {
            return "Show this message";
        }

        private string CollectAllUsage()
        {
            return EnumUtil<_Command>.Enumerate()
            .Select(s => $"{s.DisplayName()}: {s.CommandInterface().Usage()}")
            .Aggregate((i, j) => i + System.Environment.NewLine + j);
        }
    }




}