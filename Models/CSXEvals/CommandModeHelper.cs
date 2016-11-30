using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ChatBotWithWS.Models.Entities;

namespace ChatBotWithWS.Models.CSXEvals
{
    public enum BundleCommandType {
        EVAL = 0,
        RUN,
        LIST,
    };

    public class CommandModeHelper
    {
        private static List<string> BundleCommand = new List<string>{
            "eval",
            "run",
            "list",
        };

        public static ScriptEntry GenerateScriptEntry(Models.ChatCommands.CommandModel model)
        {
            const string Pattern = @"^(?<command>\S+)\s*(?<args>.*)$";
            var re = new Regex(Pattern, RegexOptions.Compiled| RegexOptions.Singleline);

            var match = re.Match(model.Args);
            if (!match.Success) return null;

            if (!BundleCommand.Contains(match.Groups["command"].Value.ToLower())) return null;
            
            var type_idx = BundleCommand.IndexOf(match.Groups["command"].Value.ToLower());

            var entry = new ScriptEntry();
            entry.CommandType = (BundleCommandType)type_idx;

            if (entry.CommandType == BundleCommandType.LIST) {
                entry.CommandType = BundleCommandType.LIST;
                return entry;
            }

            var args = match.Groups["args"].Value;
            var tmp = args.Split(' ').AsEnumerable();
            if (entry.CommandType == BundleCommandType.RUN) {
                entry.EvalTarget = tmp.First();
                try {
                    tmp = tmp.Skip(1);
                } catch(Exception ex) {
                    // not call, null char as is space char...
                    System.Console.WriteLine("Arg is null");
                    System.Console.WriteLine(ex.StackTrace);
                }
            } else {
                entry.ScriptBody = model.Code;
            }

            entry.Args = tmp?.ToArray();

            return entry;
        }
    }
}