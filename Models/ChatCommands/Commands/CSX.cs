using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using ChatBotWithWS.Models;
using ChatBotWithWS.Models.Entities;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ChatBotWithWS.Models.CSXEvals;

namespace ChatBotWithWS.Models.ChatCommands.Commands
{
    public class CSX : IChatCommand
    {
        private Services.ICSXEvalService EvalService{get;set;} = (Services.ICSXEvalService)ChatBotWithWS.Startup.Services.GetService(typeof(Services.ICSXEvalService));
        public async Task<ChatTransferModel> Run(CommandModel model)
        {
            var trans = new Models.Entities.ChatTransferModel();
            trans.MessageType = ChatMessageType.Bot;

            var entry = CommandModeHelper.GenerateScriptEntry(model);
            
            if (entry == null) {
                trans.Text = Usage();
                trans.Success = false;
                return trans;
            }

            switch(entry.CommandType)
            {
                case BundleCommandType.EVAL:
                var result = await EvalService.SafeScriptEval(entry);
                trans.Success = result.Success;
                trans.Text = result.Result;
                break;
                case BundleCommandType.RUN:
                trans.Success = false;
                trans.Text = "Un supported";
                break;
                case BundleCommandType.LIST:
                trans.Success = false;
                trans.Text = "Un supported";
                break;
            }
            
            return trans;
        }

        public string Usage()
        {
            return @"eval [args] => when use playground(require json field 'code')
                    run <commandName> [args] => when use saved command
                    list => list saved command";
        }
    }
}