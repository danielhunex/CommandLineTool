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
                bool silent = command.Flags.TryGetValue("-silent", out var slt);

                if (command.Flags.TryGetValue("-s", out var single))
                {
                    var programInfo = GetProgramInfo(command.Args.ToArray()[0], programs);
                    Uninstall(programInfo, command, silent);
                }
                else if (command.Flags.TryGetValue("-m", out var multiple))
                {
                    foreach (var arg in command.Args)
                    {
                        var programInfo = GetProgramInfo(arg, programs);
                        Uninstall(programInfo, command, silent, arg);
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

        private ProgramInfo GetProgramInfo(string programName, SortedSet<ProgramInfo> programs)
        {
            if (!programName.Contains("*"))
            {
                return programs.Where(pr => pr.Name.ToLower().Equals(programName.ToLower(), StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            }
            if (programName.StartsWith("*"))
            {
                var partOfProgramName = programName.Substring(1);
                return programs.Where(pr => pr.Name.ToLower().Contains(partOfProgramName.ToLower())).FirstOrDefault();
            }
            if (programName.EndsWith("*"))
            {
                var partOfProgramName = programName.Substring(0, programName.Length - 1);
                return programs.Where(pr => pr.Name.ToLower().Contains(partOfProgramName.ToLower())).FirstOrDefault();
            }
            int wildCardIndex = programName.IndexOf("*");
            var firstPartName = programName.Substring(0, wildCardIndex);
            var secondPartName = programName.Substring(wildCardIndex + 1);
            return programs.Where(pr => pr.Name.ToLower().Contains(firstPartName.ToLower()) && pr.Name.ToLower().Contains(secondPartName.ToLower())).FirstOrDefault();
        }

        private void Uninstall<T>(ProgramInfo info, T command, bool silent = false, string arg = null) where T : ICommand
        {
            try
            {
                if (info == null)
                {
                    _logger.LogWarning($"Unable to find the programe: {arg}. You need to type the full name of the application or use wildcard (*). {command.ToHelp()}");
                }
                else
                {
                    var process = new Process();
                    string argument;
                    string exe;

                    if (!info.IsExe)
                    {
                        string silentFlag = silent ? "/qn" : "";
                        exe = "msiexec.exe";
                        argument = $"{info.UninstallInfo.ToLower().Replace("msiexec.exe", "")} {silentFlag}";
                    }
                    else
                    {
                        int endOfExeIndex = info.UninstallInfo.ToLower().IndexOf(".exe") + 5;
                        exe = info.UninstallInfo.Substring(0, endOfExeIndex);
                        argument = info.UninstallInfo.Substring(endOfExeIndex);
                        process.StartInfo.CreateNoWindow = silent;
                    }

                    process.StartInfo.FileName = exe;
                    process.StartInfo.Arguments = argument;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.ErrorDialog = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;

                    process.ErrorDataReceived += (sendingProcess, errorLine) => { if (!string.IsNullOrWhiteSpace(errorLine.Data)) {_logger.LogInformation(errorLine.Data); } };
                    process.OutputDataReceived += (sendingProcess, dataLine) => { if (!string.IsNullOrWhiteSpace(dataLine.Data)) { _logger.LogInformation(dataLine.Data); } };

                    process.Start();
                    process.BeginErrorReadLine();
                    process.BeginOutputReadLine(); 

                    process.WaitForExit();



                }
            }
            catch (Exception ex)
            {

                _logger.LogError($"{ex.Message} {NewLine} {command.ToHelp()}");
            }
        }
    }
}
