using System.Collections.Generic;

namespace CmdTool.Core
{
    public interface ICommand
    {
        string Name { get; }
        SortedSet<string> Flags { get; }
        SortedSet<string> Args { get; }
        void setCommandOptions(CommandLineOption options);
        bool IsValid();
        string ToHelp();
    }
}
