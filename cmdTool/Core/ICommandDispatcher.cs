namespace CmdTool.Core.Commands
{
    public interface ICommandDispatcher
    {
        void Execute(CommandLineOption cmdOptions);
    }
}
