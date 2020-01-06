using CmdTool.Core;
using System.Collections.Generic;
using System.Text;
using static System.Environment;

namespace CmdTool
{
    public class HelpCommand : Command
    {
        public override string Name { get; } = "help";

        protected override HashSet<string> AllowedFlags => new HashSet<string>() { "-h" };

        public override bool IsValid()
        {
            if (Args != null || Args.Count > 0)
            {
                return false;
            }
            if (Flags?.Count > 1)
            {
                return false;
            }

            if (Flags?.Count == 1 && !Flags.TryGetValue("-h", out var helpFlag))
            {
                return false;
            }

            return base.IsValid();
        }
        public override string ToHelp()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"DESCRIPTION")
                   .Append(NewLine)
                   .Append("\t")
                   .Append($"{Name} is a command to see what the tool does")
                   .Append(NewLine)
                   .Append("USAGE:")
                   .Append(NewLine)
                   .Append($"\tCmdTool.exe {Name} <flag>")
                   .Append($"{NewLine} \t <flag> is optional. flag -h will show how to use the 'help' command")
                   .Append("EXAMPLE:")
                   .Append(NewLine)
                   .Append($"\tCmdTool.exe {Name}    will show the help as to how to use all the commands ")
                   .Append(NewLine)
                   .Append($"\tCmdTool.exe {Name} -h    will show the help of the 'help' command");
            return builder.ToString();
        }
    }
}
