using CmdTool.Core.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace CmdTool.Core
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IEnumerable<ICommand> _commands;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<CommandDispatcher> _logger;

        public CommandDispatcher(IServiceScopeFactory serviceScopeFactory, IEnumerable<ICommand> commands, ILogger<CommandDispatcher> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _commands = commands;
            _logger = logger;
        }

        public void Execute(CommandLineOption cmdOptions)
        {
            ICommand command = _commands.Where(cmd => cmd.Name.Equals(cmdOptions.CommandName, System.StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

            if (command == null)
            {
                _logger.LogError($"The command {cmdOptions.CommandName} does not exist. Please see the help description");
                ExecuteHelp();
            }
            else
            {
                Execute(cmdOptions, command);
            }
        }

        private void ExecuteHelp()
        {
            var command = _commands.Where(cmd => cmd.Name.Equals("help", System.StringComparison.OrdinalIgnoreCase)).First();
            var helpCommandOptions = new CommandLineOption { CommandName = "help" };
            Execute(helpCommandOptions, command);
        }

        private void Execute(CommandLineOption cmdOptions, ICommand command)
        {
            command.setCommandOptions(cmdOptions);
            var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var cmd = scope.ServiceProvider.GetRequiredService(handlerType) as ICommandHandler;
                cmd.Execute(command);
            };
        }
    }
}
