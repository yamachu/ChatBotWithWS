using System;

namespace ChatBotWithWS.Models.CSXEvals
{
    public class ScriptEntry
    {
        public BundleCommandType CommandType;
        public string[] Args;
        public string ScriptBody;
        public string EvalTarget;
        
    }
}