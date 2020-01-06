using CmdTool.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.Environment;
namespace CmdTool
{
    public class HelpCommandHandler : ICommandHandler<HelpCommand>
    {
        private IEnumerable<ICommand> _commands;
        private ILogger<HelpCommand> _logger;
        public HelpCommandHandler(IEnumerable<ICommand> commands, ILogger<HelpCommand> logger)
        {
            _commands = commands;
            _logger = logger;
        }

        public void Execute<T>(T command) where T : ICommand
        {
            string flag = command?.Flags?.ToArray().FirstOrDefault();

            if (flag != null && flag.Equals("-h", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation($"{NewLine} {command.ToHelp()}");
            }
            else
            {
                StringBuilder builder = new StringBuilder();
                foreach (var cmd in _commands)
                {
                    builder.Append($"{NewLine}")
                           .Append($"{cmd.ToHelp()}");
                }
                _logger.LogInformation(builder.ToString());
            }
        }
    }
}
