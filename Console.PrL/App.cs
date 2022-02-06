using BLL.Abstractions.Interfaces.RoleInterfaces;
using BLL.Abstractions.Interfaces.RoomInterfaces;
using BLL.Abstractions.Interfaces.UserInterfaces;
using Console.PrL.Commands;
using Console.PrL.Commands.RoomCommands;
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

        private string authToken;

        public App(ILogger<App> logger, IConsole console, IEnumerable<Command> commands)
        {
            this.logger = logger;
            this.console = console;

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
                        this.logger.LogError(ex.Message);
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
