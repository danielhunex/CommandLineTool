using System.Collections.Generic;

namespace CmdTool.Core
{
    public abstract class Command : ICommand
    {
        private CommandLineOption _options;
        public abstract string Name { get; }
        protected abstract HashSet<string> AllowedFlags { get; }
        public SortedSet<string> Flags => _options.Flags ?? new SortedSet<string>();
        public SortedSet<string> Args => _options.Args ?? new SortedSet<string>();
        public virtual bool IsValid() => Validate();
        public void setCommandOptions(CommandLineOption options) => _options = options;
        public abstract string ToHelp();
        private bool Validate()
        {
            if (Flags != null && !Flags.IsSubsetOf(AllowedFlags))
            {
                return false;
            }
            if (!Name.Equals(_options.CommandName, System.StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }
    }
}
