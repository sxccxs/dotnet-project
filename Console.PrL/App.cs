using BLL.Abstractions.Interfaces;
using Console.PrL.Commands;
using Console.PrL.Commands.UserCommands;
using Console.PrL.Interfaces;
using Microsoft.Extensions.Logging;

namespace Console.PrL
{
    public class App
    {
        private readonly IConsole console;

        private readonly ILogger logger;

        private readonly Dictionary<string, Command> commands;

        private readonly IStartupService startupService;

        private string authToken;

        public App(ILogger<App> logger, IConsole console, IStartupService startupService, IEnumerable<Command> commands)
        {
            this.logger = logger;
            this.console = console;
            this.startupService = startupService;

            this.commands = new Dictionary<string, Command>();
            var commandsList = commands.ToList();
            var helpCommand = new HelpCommand(console, commandsList);
            this.commands.Add(helpCommand.Name, helpCommand);
            foreach (var command in commandsList)
            {
                this.commands.Add(command.Name, command);
            }
        }

        public async Task StartApp()
        {
            var startupResult = await this.startupService.SetUp();
            if (!startupResult.IsSuccess)
            {
                this.logger.LogError(startupResult.ExceptionMessage);
                throw new Exception(startupResult.ExceptionMessage);
            }

            while (true)
            {
                var command = this.console.Input(string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(command))
                {
                    continue;
                }

                if (this.commands.ContainsKey(command))
                {
                    try
                    {
                        var cmd = this.commands[command];
                        var result = await cmd.Execute(this.authToken);
                        if (cmd is LoginCommand && result.IsSuccess)
                        {
                            this.authToken = result.Value;
                        }
                        else if (!result.IsSuccess)
                        {
                            this.console.Print($"{result.ExceptionMessage}");
                        }
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogError($"Message {ex.Message} Place: {ex.Source} StackTrace: {ex.StackTrace}");
                        throw;
                    }
                }
                else
                {
                    this.console.Print($"Invalid command {command}.");
                }
            }
        }
    }
}
