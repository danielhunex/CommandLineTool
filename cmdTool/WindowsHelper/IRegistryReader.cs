using System.Collections.Generic;
namespace CmdTool.WindowsHelper
{
    public interface IRegistryReader
    {
        SortedSet<ProgramInfo> GetInstalledProgramInfos();
    }
}
