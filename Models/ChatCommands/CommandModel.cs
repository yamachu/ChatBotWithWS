using System;
namespace ChatBotWithWS.Models.ChatCommands
{
    public class CommandModel
    {
        public string Target{get;set;}
        public string Command{get;set;}
        public string Args{get;set;}
        public string Code{get;set;}
    }   
}