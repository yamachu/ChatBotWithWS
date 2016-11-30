using System;
using System.Linq;
using System.Text.RegularExpressions;
using ChatBotWithWS.Models.Entities;

namespace ChatBotWithWS.Models.ChatCommands
{
    public class CommandHelper
    {
        private static string[] BotAcceptCommon = {
            "@bot",
            "bot",
        };

        private static string[] BotAcceptSpecial = {
            "bot:"
        };

        public static CommandModel isValidCommandFormat(string body)
        {
            return isValidCommonMention(body)
                ?? isValidSpecialMention(body);
        }

        public static CommandModel isValidCommandFormat(ChatRecieveModel model)
        {
            var tmp = isValidCommandFormat(model.Text);
            if (tmp != null) {
                tmp.Code = model.CSX;
            }
            return tmp;
        }

        private static CommandModel isValidCommonMention(string body)
        {
            const string Pattern = @"^(?<target>\S+)\s+(?<command>\w+)\s*(?<args>.*)$";
            var re = new Regex(Pattern, RegexOptions.Compiled| RegexOptions.Singleline);

            var match = re.Match(body);
            if (!match.Success) return null;

            if (!BotAcceptCommon.Contains(match.Groups["target"].Value.ToLower())) return null;
            
            var command = new CommandModel()
            {
                Target = match.Groups["target"].Value.ToLower(),
                Command = match.Groups["command"].Value.ToLower(),
                Args = match.Groups["args"]?.Value?.ToLower(),
            };

            return command;
        }

        private static CommandModel isValidSpecialMention(string body)
        {
            const string Pattern = @"^(?<target>\S+:)\s*(?<command>\w+)\s*(?<args>.*)$";
            // SingleLineにすると改行を許す -> csx用
            var re = new Regex(Pattern, RegexOptions.Compiled| RegexOptions.Singleline);

            var match = re.Match(body);
            if (!match.Success) return null;

            if (!BotAcceptSpecial.Contains(match.Groups["target"].Value.ToLower())) return null;
            
            var command = new CommandModel()
            {
                Target = match.Groups["target"].Value.ToLower(),
                Command = match.Groups["command"].Value.ToLower(),
                Args = match.Groups["args"]?.Value?.ToLower()
            };

            return command;
        }         
    }
}