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

                try
                {
                    if (this.commands.ContainsKey(command))
                    {
                        var cmd = this.commands[command];
                        if (cmd is LoginCommand)
                        {
                            this.authToken = cmd.Execute().Value;
                        }
                        else
                        {
                            cmd.Execute();
                        }
                    }
                    else
                    {
                        this.console.Print($"Invalid command {command}.\n");
                    }
                }
                catch (Exception ex)
                {
                    this.console.Print($"{ex.Message}\n");
                }
            }
        }
    }
}
