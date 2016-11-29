using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using ChatBotWithWS.Models;
using ChatBotWithWS.Models.Entities;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ChatBotWithWS.Models.ChatCommands.Commands
{
    public class CSX : IChatCommand
    {
        private Services.ICSXEvalService EvalService{get;set;} = (Services.ICSXEvalService)ChatBotWithWS.Startup.Services.GetService(typeof(Services.ICSXEvalService));
        public async Task<ChatTransferModel> Run(CommandModel model)
        {
            var trans = new Models.Entities.ChatTransferModel();
            trans.MessageType = ChatMessageType.Bot; 

            var result = await EvalService.SafeScriptEval(
                new Models.CSXEvals.ScriptEntry(){
                    ScriptBody = "1+1+int.Parse(Args.ToArray()[0])"
                },
                model);

            trans.Success = result.Success;
            trans.Text = result.Result;

            return trans;
        }

        public string Usage()
        {
            return "<commadName> [args]";
        }
    }
}