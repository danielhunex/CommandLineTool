namespace CmdTool.Core
{
    public interface ISimpleCommandParser
    {
        CommandLineOption Parse(string[] args);
    }
}
