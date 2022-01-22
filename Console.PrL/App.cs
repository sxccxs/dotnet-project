using BLL.Abstractions.Interfaces.UserInterfaces;
using Console.PrL.Commands;
using Console.PrL.Interfaces;

namespace Console.PrL
{
    public class App
    {
        private readonly IConsole console;

        private readonly Dictionary<string, Command> commands;

        private string authToken;

        public App(
            IConsole console,
            ILoginService loginService,
            IRegistrationService registrationService,
            IAccountActivationService accountActivationService)
        {
            this.console = console;

            var commandsArray = new Command[]
            {
                new LoginCommand(console, loginService),
                new RegistrationCommand(console, registrationService),
                new ActivationCommand(console, accountActivationService),
            };

            this.commands = new Dictionary<string, Command>();
            foreach (var command in commandsArray)
            {
                this.commands.Add(command.Name, command);
            }
        }

        public void StartApp()
        {
            while (true)
            {
                var command = this.console.Input(string.Empty).Trim();
                if (command is null || string.IsNullOrWhiteSpace(command))
                {
                    continue;
                }

                if (this.commands.ContainsKey(command))
                {
                    var cmd = this.commands[command];
                    var result = cmd.Execute();
                    if (cmd is LoginCommand && result.IsSuccess)
                    {
                        this.authToken = result.Value;
                    }
                    else if (!result.IsSuccess)
                    {
                        this.console.Print($"{result.ExceptionMessage}\n");
                    }
                }
                else
                {
                    this.console.Print($"Invalid command {command}.\n");
                }
            }
        }
    }
}
