using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using ChatBotWithWS.Models;
using System.Threading.Tasks;
namespace ChatBotWithWS.Models.ChatCommands
{
    public enum _Command {
        PING,
        CSX,
        HELP,
    }

    static class _CommandExt
    {
        static List<Tuple<string, Type>> commands = new List<Tuple<string, Type>> {
            {"ping", typeof(Commands.Ping)},
            {"csx", typeof(Commands.CSX)},
            {"help", typeof(Commands.Help)}
        };

        public static string DisplayName(this _Command command)
        {
            return commands[(int)command].Item1;
        }

        public static IChatCommand CommandInterface(this _Command command)
        {
            return Activator.CreateInstance(commands[(int)command].Item2) as IChatCommand;
        }
    }

    public class CommandRunner
    {
        private static bool isContains(string command_str)
        {
            return EnumUtil<_Command>.Enumerate()
            .Count(s => s.DisplayName() == command_str) > 0;
        }

        public async static Task<Models.Entities.ChatTransferModel> GenerateResponse(CommandModel model)
        {
            if (!isContains(model.Command)) {
                return await new Models.ChatCommands.Commands.Undefined().Run(model);
            } 
                
            var target = EnumUtil<_Command>.Enumerate()
                .First(s => s.DisplayName() == model.Command);

            return await target.CommandInterface().Run(model);
        }
    }


}
