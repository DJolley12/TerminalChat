using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace TerminalChatLib
{
    public static class Helpers
    {
        public static ListenMethodType ParseListenMethodType(string typeString)
        {
            Console.WriteLine(typeString);
            switch (typeString)
            {
                case "Whitelist":
                    return ListenMethodType.Whitelist;
                case "Blacklist":
                    return ListenMethodType.Blacklist;
                case "AllowAll":
                    return ListenMethodType.AllowAll;
                default:
                    throw new Exception($"Unable to parse {typeString}");
            }
        }
    }
}
