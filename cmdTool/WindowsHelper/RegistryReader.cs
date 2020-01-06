using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static System.Environment;
namespace CmdTool.WindowsHelper
{
    public class RegistryReader : IRegistryReader
    {
        private readonly ILogger<RegistryReader> _logger;
        private List<string> REGISTRY_KEYS = new List<string>()
        {
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
            @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
        };

        public RegistryReader(ILogger<RegistryReader> logger)
        {
            _logger = logger;
        }

        public SortedSet<ProgramInfo> GetInstalledProgramInfos()
        {
            var fromLocalMachine = GetInstalledProgramInfos(RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64), REGISTRY_KEYS);
            var fromCurrentUser = GetInstalledProgramInfos(RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64), REGISTRY_KEYS);
            fromLocalMachine.UnionWith(fromCurrentUser);
            return fromLocalMachine;
        }

        private SortedSet<ProgramInfo> GetInstalledProgramInfos(RegistryKey regKey, List<string> keys)
        {
            SortedSet<ProgramInfo> installedPrograms = new SortedSet<ProgramInfo>();
            foreach (string key in keys)
            {
                using (RegistryKey rk = regKey.OpenSubKey(key))
                {
                    if (rk != null)
                    {
                        foreach (string skName in rk.GetSubKeyNames())
                        {
                            using (RegistryKey sk = rk.OpenSubKey(skName))
                            {
                                try
                                {
                                    string programName = Convert.ToString(sk.GetValue("DisplayName"));
                                    string uninstallInfo = Convert.ToString(sk.GetValue("UninstallString"));
                                    if (!string.IsNullOrWhiteSpace(programName) && !string.IsNullOrWhiteSpace(uninstallInfo))
                                    {
                                        var (isExe, uninstallString) = ParseUninstallString(uninstallInfo);
                                        installedPrograms.Add(
                                            new ProgramInfo
                                            {
                                                Name = programName.ToLower(),
                                                UninstallInfo = uninstallString,
                                                IsExe = isExe
                                            });
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError($"Error occurred while trying to read installed program. Make sure that you are running under elevated mode {NewLine} {ex.Message}");
                                }
                            }
                        }
                    }
                }

            }
            return installedPrograms;
        }

        private (bool, string) ParseUninstallString(string uninstallstring)
        {
            bool isExe = false;
            string modifiedUinstall;
            if (uninstallstring.Substring(0, 1).Equals("\"") |
                uninstallstring.ToLower().Contains("msiexec") |
                uninstallstring.Contains("~"))
            {
                modifiedUinstall = uninstallstring;
            }
            else if (uninstallstring.ToLower().IndexOf(".exe") > 0)
            {
                modifiedUinstall = "\"" + uninstallstring.Insert(uninstallstring.ToLower().IndexOf(".exe") + 4, "\"");
                isExe = true;
            }
            else
            {
                modifiedUinstall = "\"" + uninstallstring + "\"";
            }
            return (isExe, modifiedUinstall);
        }
    }

    public class ProgramInfo : IComparable<ProgramInfo>
    {
        public string Name { get; set; }
        public string UninstallInfo { get; set; }
        public bool IsExe { get; set; }
        public int CompareTo([AllowNull] ProgramInfo other)
        {
            return Name.CompareTo(other?.Name);
        }
    }
}
