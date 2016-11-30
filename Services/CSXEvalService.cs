using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ChatBotWithWS.Models.ChatCommands;
using ChatBotWithWS.Models.CSXEvals;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ChatBotWithWS.CSXEvals.Global
{
    public class Globals
    {
        public Globals(string[] args)
        {
            Args = args;
        }
        public string[] Args {get; private set;}
    }
}

namespace ChatBotWithWS.Services
{
    public interface ICSXEvalService
    {
        void DebugLog(string message);
        Task HandleConnection(HttpContext context);
        Task<EvalResult> SafeScriptEval(ScriptEntry entry);
    }

    public class CSXEvalService : ICSXEvalService
    {
        private static readonly int TIMEOUT = 5000;
        private static readonly string[] DefaultImports = 
        {
            "System",
            "System.IO",
            "System.Linq",
            "System.Collections.Generic",
            "System.Text",
            "System.Text.RegularExpressions",
            "System.Net",
            "System.Net.Http",
            "System.Threading",
            "System.Threading.Tasks",
            "Newtonsoft.Json",
        };

        private static readonly Assembly[] DefaultReferences =
        {
            typeof(Enumerable).GetTypeInfo().Assembly,
            typeof(List<string>).GetTypeInfo().Assembly,
            typeof(System.Text.RegularExpressions.Regex).GetTypeInfo().Assembly,
            typeof(System.Net.Http.HttpClient).GetTypeInfo().Assembly,
            typeof(Newtonsoft.Json.JsonConverter).GetTypeInfo().Assembly,
        };

        private static readonly ScriptOptions DefaultOptions = 
            ScriptOptions.Default
                        .WithImports(DefaultImports)
                        .WithReferences(new [] {
                            "System",
                            "System.Core",
                            "System.Xml",
                            "System.Xml.Linq",
                        })
                        .WithReferences(DefaultReferences);

        public void DebugLog(string message)
        {
            throw new NotImplementedException();
        }

        public Task HandleConnection(HttpContext context)
        {
            throw new NotImplementedException();
        }

        // support description or usage?
        public async Task<EvalResult> SafeScriptEval(ScriptEntry entry)
        {
            if (entry.ScriptBody == null || entry.ScriptBody == "") return new EvalResult()
            {
                Result = "コードが空です",
                Success = false,
            }; 

            var globals = new ChatBotWithWS.CSXEvals.Global.Globals(entry.Args);

            var cts = new CancellationTokenSource();

            var evalTask = Task<EvalResult>.Run(async () => {
                    EvalResult result;
                    try
                    {
                        object tmp = await CSharpScript.EvaluateAsync(entry.ScriptBody, DefaultOptions, globals, typeof(ChatBotWithWS.CSXEvals.Global.Globals));

                        result = new EvalResult()
                        {
                            Result = tmp?.ToString() ?? "",
                            Success = true
                        };
                    }
                    catch (Exception ex)
                    {
                        result = new EvalResult()
                        {
                             Result = $"{ex.Message}{Environment.NewLine}{ex.StackTrace}",
                             Success = false
                        };
                    }
                    return result;
                }, cts.Token);

            if (evalTask == await Task.WhenAny(evalTask, Task.Delay(TIMEOUT, cts.Token)))
            {
                var result = await evalTask;
                cts.Cancel();
                return result;
            }
            else
            {
                cts.Cancel();
                return new EvalResult()
                    {
                        Result = "Timeout",
                        Success = false
                    };
            }
        }
    }
}