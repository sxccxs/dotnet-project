using BLL.Abstractions.Interfaces.UserInterfaces;
using Console.PrL.Interfaces;
using Core.DataClasses;
using Core.Models.UserModels;

namespace Console.PrL.Commands
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

        public override OptionalResult<string> Execute(string token)
        {
            var userData = this.GetLoginInfo();
            var tokenResult = this.loginService.Login(userData);
            if (tokenResult.IsSuccess)
            {
                this.Console.Print("Logged in successfully\n");
            }

            return tokenResult;
        }

        private UserLoginModel GetLoginInfo()
        {
            this.Console.Print("\n");
            var userLoginModel = new UserLoginModel
            {
                Email = this.Console.Input("Email: "),
                Password = this.Console.Input("Password: "),
            };
            this.Console.Print("\n");
            return userLoginModel;
        }
    }
}
