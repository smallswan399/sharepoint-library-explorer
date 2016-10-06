using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;

namespace Main.Core
{
    public class CmdHelper
    {
        public static string GetString(string[] args)
        {
            var argStr = "";
            for (int i = 0; i < args.Length; i++)
            {
                argStr += args[i];
                if (i < args.Length - 1)
                {
                    argStr += ", ";
                }
            }
            return argStr;
        }
    }

    class CmdParameters
    {
        [Option('h', "handle", Required = false, HelpText = "Owner windows hander")]
        public long? Handle { get; set; }
    }
}
