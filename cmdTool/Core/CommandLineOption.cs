using System.Collections.Generic;

namespace CmdTool.Core
{
    public class CommandLineOption
    {
        public string CommandName { get; set; }
        public SortedSet<string> Flags { get; set; }
        public SortedSet<string> Args { get; set; }
    }
}
