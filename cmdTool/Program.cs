using CmdTool.Core;
using CmdTool.Core.Commands;
using CmdTool.WindowsHelper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var services = serviceCollection.BuildServiceProvider();

            using (services)
            {
                var logger = services.GetService<ILogger>();
                var commandParser = services.GetService<ISimpleCommandParser>();
                var options = commandParser.Parse(args);
                var commandDispatcher = services.GetService<ICommandDispatcher>();
                commandDispatcher.Execute(options);
            }
        }

        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(x => x.AddConsole());
            services.Scan(svc => svc.FromAssemblyOf<ICommand>()
            .AddClasses(c => c.AssignableTo<ICommand>())
            .AsImplementedInterfaces()
            .WithTransientLifetime());

            services.AddSingleton<ISimpleCommandParser, SimpleCommandParser>();
            services.AddSingleton<IRegistryReader, RegistryReader>();
            services.AddSingleton<ICommandDispatcher, CommandDispatcher>();
            services.Scan(svc => svc.FromAssemblyOf<ICommandHandler>()
                  .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>)))
                  .AsImplementedInterfaces()
                  .WithScopedLifetime());
        }
    }
}

