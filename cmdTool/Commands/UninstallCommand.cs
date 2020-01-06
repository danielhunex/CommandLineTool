using CmdTool.Core;
using System.Collections.Generic;
using System.Text;
using static System.Environment;

namespace CmdTool
{
    public class UninstallCommand : Command
    {
        public override string Name { get; } = "uninstall";
        protected override HashSet<string> AllowedFlags => new HashSet<string> { "-s", "-m", "-help", "-silent" };

        public override bool IsValid()
        {
            if (Flags == null)
            {
                return false;
            }
            if (Flags?.Count > 2)
            {
                return false;
            }

            if (Flags?.Count == 2 && !Flags.TryGetValue("-silent", out var silentFlag))
            {
                return false;
            }

            if (Flags?.Count == 2 && Flags.TryGetValue("-help", out var helpFlag))
            {
                return false;
            }

            if (Flags?.Count == 2 && Args?.Count == 0)
            {
                return false;
            }
            if (Flags.TryGetValue("-help", out helpFlag) && Args.Count > 0)
            {
                return false;
            }

            if (Flags.TryGetValue("-s", out var singleFlag) && Args.Count != 1)
            {
                return false;
            }

            if (Flags.TryGetValue("-m", out var multipleFlag) && Args.Count <= 1)
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
                   .Append($"{Name} is a command used for uninstalling softwares")
                   .Append(NewLine)
                   .Append("USAGE:")
                   .Append(NewLine)
                   .Append($"\tCmdTool.exe {Name} <flags> [software names]")
                   .Append(NewLine)
                   .Append("\t[software names]: the full names of the softwares to be uninstalled.")
                   .Append($"{NewLine}\t\tMultiword names should be in quotes (see the example below)")
                   .Append($"{NewLine}\t\t Only single wildcard (*) is supported in the names. Wildcard can be at start, end or in the middle")
                   .Append($"{NewLine}\t<flags>: -s (single) -m (multiple)")
                   .Append(NewLine)
                   .Append("EXAMPLE:")
                   .Append(NewLine)
                   .Append($"\tCmdTool.exe {Name} -s Zoom    This will uninstall Zoom")
                   .Append($"{NewLine}")
                   .Append($"\tCmdTool.exe {Name} -s -silent \"Python Laucher\"    This will uninstall 'Python Laucher' silently")
                   .Append(NewLine)
                   .Append($"\tCmdTool.exe {Name} -m \"Notepad++*\" Zoom    This will uninstall Notepad++ and Zoom");
            return builder.ToString();
        }
    }
}
