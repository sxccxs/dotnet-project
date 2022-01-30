using BLL.Abstractions.Interfaces.UserInterfaces;
using Console.PrL.Interfaces;
using Core.DataClasses;

namespace Console.PrL.Commands.UserCommands
{
    internal class DeleteAccountCommand : Command
    {
        private readonly IAuthenticationService authenticationService;

        private readonly IDeleteUserService deleteService;

        public DeleteAccountCommand(IConsole console, IAuthenticationService authenticationService, IDeleteUserService deleteService)
            : base(console)
        {
            this.authenticationService = authenticationService;
            this.deleteService = deleteService;
        }

        public override string Name => "/deleteAccount";

        public override string Description => "Deletes your account";

        public async override Task<OptionalResult<string>> Execute(string token)
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

            var deleteResult = await this.deleteService.DeleteUser(userResult.Value);
            if (!deleteResult.IsSuccess)
            {
                return new OptionalResult<string>(deleteResult);
            }

            this.Console.Print("Deleted successfully");

            return new OptionalResult<string>(deleteResult);
        }

        private bool Ask()
        {
            var input = this.Console.Input("Are you sure you want to delete your account?[y/N]").ToLower();
            return (new string[] { "yes", "y" }).Contains(input);
        }
    }
}
