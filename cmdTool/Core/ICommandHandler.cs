namespace CmdTool.Core
{
    public interface ICommandHandler
    {
        void Execute<T>(T command) where T : ICommand;
    }
    public interface ICommandHandler<in T> : ICommandHandler where T : ICommand
    {

    }
}
