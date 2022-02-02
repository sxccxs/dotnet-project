using BLL.Abstractions.Interfaces.UserInterfaces;
using Console.PrL.Interfaces;
using Core.DataClasses;
using Core.Models.UserModels;

namespace Console.PrL.Commands.UserCommands
{
    internal class LoginCommand : Command
    {
        private readonly ILoginService loginService;

        public LoginCommand(IConsole console, ILoginService loginService)
            : base(console)
        {
            this.loginService = loginService;
        }

        public override string Name => "/login";

        public override string Description => "Login to app";

        public async override Task<OptionalResult<string>> Execute(string token)
        {
            var userData = this.GetLoginInfo();
            var tokenResult = await this.loginService.Login(userData);
            if (tokenResult.IsSuccess)
            {
                this.Console.Print("Logged in successfully");
            }

            return tokenResult;
        }

        private UserLoginModel GetLoginInfo()
        {
            this.Console.Print();
            var userLoginModel = new UserLoginModel
            {
                Email = this.Console.Input("Email: "),
                Password = this.Console.Input("Password: "),
            };
            this.Console.Print();
            return userLoginModel;
        }
    }
}
