using BLL.Abstractions.Interfaces.RoomInterfaces;
using BLL.Abstractions.Interfaces.UserInterfaces;
using Console.PrL.Commands;
using Console.PrL.Commands.RoomCommands;
using Console.PrL.Commands.UserCommands;
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
            IAccountActivationService accountActivationService,
            IAuthenticationService authenticationService,
            IUserRoomService userRoomService)
        {
            this.console = console;

            var commandsArray = new Command[]
            {
                new LoginCommand(console, loginService),
                new RegistrationCommand(console, registrationService),
                new ActivationCommand(console, accountActivationService),
                new MeCommand(console, authenticationService),
                new GetRoomsCommand(console, authenticationService, userRoomService),
                new CreateRoomCommand(console, authenticationService, userRoomService),
                new UpdateRoomCommand(console, authenticationService, userRoomService),
                new DeleteRoomCommand(console, authenticationService, userRoomService),
            };

            commandsArray = commandsArray.Append(new HelpCommand(console, commandsArray)).ToArray();

            this.commands = new Dictionary<string, Command>();
            foreach (var command in commandsArray)
            {
                this.commands.Add(command.Name, command);
            }
        }

        public async Task StartApp()
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
                    var result = await cmd.Execute(this.authToken);
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
