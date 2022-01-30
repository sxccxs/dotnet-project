using BLL.Abstractions.Interfaces.UserInterfaces;
using Console.PrL.Interfaces;
using Core.DataClasses;

namespace Console.PrL.Commands.UserCommands
{
    internal class DeleteUserCommand : Command
    {
        private readonly IDeleteUserService deleteUserService;
        private readonly IAuthenticationService authenticationService;

        public DeleteUserCommand(
            IConsole console,
            IAuthenticationService authenticationService,
            IDeleteUserService deleteUserService)
            : base(console)
        {
            this.authenticationService = authenticationService;
            this.deleteUserService = deleteUserService;
        }

        public override string Name => "/deleteuser";

        public override string Description => "Delete the user";

        public async override Task<OptionalResult<string>> Execute(string token)
        {
            var userResult = await this.authenticationService.GetUserByToken(token);
            if (!userResult.IsSuccess)
            {
                return new OptionalResult<string>(userResult);
            }

            var user = userResult.Value;

            var result = await this.deleteUserService.DeleteUser(user.Id);
            if (result.IsSuccess)
            {
               this.Console.Print("User Deleted Successfully.");
            }

            return result;
        }
    }
}
