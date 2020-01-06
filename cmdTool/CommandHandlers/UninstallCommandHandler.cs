using CmdTool.Core;
using CmdTool.WindowsHelper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using static System.Environment;
using System.Linq;
using System.Diagnostics;

namespace CmdTool
{
    public class UninstallCommandHandler : ICommandHandler<UninstallCommand>
    {
        private readonly IRegistryReader _registryReader;
        private readonly ILogger<UninstallCommand> _logger;

        public UninstallCommandHandler(IRegistryReader registryReader, ILogger<UninstallCommand> logger)
        {
            _registryReader = registryReader;
            _logger = logger;
        }

        public void Execute<T>(T command) where T : ICommand
        {
            if (command.IsValid())
            {
                SortedSet<ProgramInfo> programs;
                try
                {
                    programs = _registryReader.GetInstalledProgramInfos();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error occurred while trying to read installed program. Make sure that you are running under elevated mode {NewLine} {ex.Message}");
                    return;
                }

                string flag = command.Flags.ToArray()[0];

                if (flag.Equals("-s", StringComparison.OrdinalIgnoreCase))
                {
                    var programInfo = programs.Where(pr => pr.Name.ToLower().Contains(command.Args.ToArray()[0].ToLower())).FirstOrDefault();
                    Uninstall(programInfo, command);
                }
                else if (flag.Equals("-m", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var arg in command.Args)
                    {
                        var programInfo = programs.Where(pr => pr.Name.ToLower().Contains(arg.ToLower())).FirstOrDefault();
                        Uninstall(programInfo, command, arg);
                    }
                }
                else
                {
                    _logger.LogInformation(command.ToHelp());
                }
            }
            else
            {
                string invalidCommandUsage = $"Invalid command Usage. Please see the description below {NewLine}";
                _logger.LogWarning($"{invalidCommandUsage} {command.ToHelp()}");
            }
        }

        private void Uninstall<T>(ProgramInfo info, T command, string arg = null) where T : ICommand
        {
            try
            {
                if (info == null)
                {
                    _logger.LogWarning($"Unable to find the programe: {arg}. You need to type the full name of the application. {command.ToHelp()}");
                }
                else
                {
                    var process = new Process();

                    if (!info.IsExe)
                    {
                        process.StartInfo.FileName = "msiexec.exe";
                        process.StartInfo.Arguments = info.UninstallInfo.ToLower().Replace("msiexec.exe", "");
                    }
                    else
                    {
                        int endOfExeIndex = info.UninstallInfo.ToLower().IndexOf(".exe") + 5;
                        string exe = info.UninstallInfo.Substring(0, endOfExeIndex);
                        string argument = info.UninstallInfo.Substring(endOfExeIndex);
                        process.StartInfo.FileName = exe;
                        process.StartInfo.Arguments = argument;
                    }
                    process.Start();
                }
            }
            catch (Exception ex)
            {

                _logger.LogError($"{ex.Message} {NewLine} {command.ToHelp()}");
            }
        }
    }
}
