using System;
using ChatBotWithWS.Models.Entities;
namespace ChatBotWithWS.Models.ChatCommands
{
    public interface IChatCommand
    {
        ChatTransferModel Run(CommandModel model);
        string Usage();
    }
}