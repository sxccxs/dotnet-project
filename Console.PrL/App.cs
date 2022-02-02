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

        public App(
            ILogger<App> logger,
            IConsole console,
            ILoginService loginService,
            IRegistrationService registrationService,
            IAccountActivationService accountActivationService,
            IAuthenticationService authenticationService,
            IEditUserInfoService editUserInfoService,
            IUserService userService,
            IUserRoomService userRoomService,
            IUserRoomRoleService userRoomRoleService)
        {
            this.logger = logger;
            this.console = console;

            var commandsArray = new Command[]
            {
                new LoginCommand(console, loginService),
                new RegistrationCommand(console, registrationService),
                new ActivationCommand(console, accountActivationService),
                new MeCommand(console, authenticationService),
                new EditAccountCommand(console, authenticationService, editUserInfoService),
                new DeleteAccountCommand(console, authenticationService, userService),
                new GetRoomsCommand(console, authenticationService, userRoomService),
                new CreateRoomCommand(console, authenticationService, userRoomService),
                new UpdateRoomCommand(console, authenticationService, userRoomService),
                new DeleteRoomCommand(console, authenticationService, userRoomService),
                new DeleteUserFromRoomCommand(console, userRoomService, authenticationService),
                new AddUserToRoomCommand(console, authenticationService, userRoomService),
                new ChangeRoleCommand(console, authenticationService, userRoomService, userRoomRoleService),
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
