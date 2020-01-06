using System.Collections.Generic;
namespace CmdTool.Core
{
    public class SimpleCommandParser : ISimpleCommandParser
    {
        public CommandLineOption Parse(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                return null;
            }
            var options = new CommandLineOption()
            {
                Args = new SortedSet<string>(),
                Flags = new SortedSet<string>()
            };

            var commandName = args[0];
            options.CommandName = commandName;

            for (int i = 1; i < args.Length; i++)
            {
                var argOrFlag = args[i];
                if (!string.IsNullOrWhiteSpace(argOrFlag))
                {
                    if (argOrFlag.StartsWith("-"))
                    {
                        options.Flags.Add(argOrFlag);
                    }
                    else
                    {
                        options.Args.Add(argOrFlag);
                    }
                }
            }

            return options;
        }
    }
}
