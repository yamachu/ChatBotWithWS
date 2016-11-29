using System;
using System.Threading.Tasks;
using ChatBotWithWS.Models.Entities;
namespace ChatBotWithWS.Models.ChatCommands
{
    public interface IChatCommand
    {
        Task<ChatTransferModel> Run(CommandModel model);
        string Usage();
    }
}