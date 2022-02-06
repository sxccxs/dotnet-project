using BLL.Abstractions.Interfaces.RoomInterfaces;
using BLL.Abstractions.Interfaces.UserInterfaces;
using Console.PrL.Interfaces;
using Core.DataClasses;

namespace Console.PrL.Commands.UserCommands
{
    internal class DeleteAccountCommand : Command
    {
        private readonly IAuthenticationService authenticationService;

        private readonly IUserRoomService userRoomService;

        public DeleteAccountCommand(IConsole console, IAuthenticationService authenticationService, IUserRoomService userRoomService)
            : base(console)
        {
            this.authenticationService = authenticationService;
            this.userRoomService = userRoomService;
        }

        public override string Name => "/deleteAccount";

        public override string Description => "Deletes your account";

        public override async Task<OptionalResult<string>> Execute(string token)
        {
            var userResult = await this.authenticationService.GetUserByToken(token);
            if (!userResult.IsSuccess)
            {
                return new OptionalResult<string>(userResult);
            }

            if (!this.Ask())
            {
                this.Console.Print("Operation cancelled");
                return new OptionalResult<string>();
            }

            var deleteResult = await this.userRoomService.DeleteUserAndRooms(userResult.Value);
            if (!deleteResult.IsSuccess)
            {
                return new OptionalResult<string>(deleteResult);
            }

            this.Console.Print("Deleted successfully");

            return new OptionalResult<string>(deleteResult);
        }

        private bool Ask()
        {
            var input = this.Console.Input("Are you sure you want to delete your account?[y/N]: ").ToLower();
            return (new string[] { "yes", "y" }).Contains(input);
        }
    }
}
