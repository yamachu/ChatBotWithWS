using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using ChatBotWithWS.Models;
namespace ChatBotWithWS.Models.ChatCommands
{
    public enum _Command {
        PING,
        HELP,
    }

    static class _CommandExt
    {
        static List<IChatCommand> CommandInterfaces;
        public static string DisplayName(this _Command command)
        {
            string[] commands = {
                "ping",
                "help"
            };

            return commands[(int)command];
        }

        public static IChatCommand CommandInterface(this _Command command)
        {
            return CommandInterfaces[(int)command];
        }

        static _CommandExt()
        {
            CommandInterfaces = new List<IChatCommand>();
            CommandInterfaces.Add(new Models.ChatCommands.Commands.Ping());
            CommandInterfaces.Add(new Models.ChatCommands.Commands.Help());
        }
    }

    public class CommandRunner
    {
        private static bool isContains(string command_str)
        {
            return EnumUtil<_Command>.Enumerate()
            .Count(s => s.DisplayName() == command_str) > 0;
        }

        public static Models.Entities.ChatTransferModel GenerateResponse(CommandModel model)
        {
            // gen help model...
            if (!isContains(model.Command)) {
                return new Models.ChatCommands.Commands.Undefined().Run(model);
            } 
                
            var target = EnumUtil<_Command>.Enumerate()
                .First(s => s.DisplayName() == model.Command);

            return target.CommandInterface().Run(model);
        }
    }


}
