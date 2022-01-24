using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces.UserInterfaces;
using Console.PrL.Interfaces;
using Core.DataClasses;
using Core.Models.UserModels;

namespace Console.PrL.Commands.UserCommands
{
    internal class MeCommand : Command
    {
        private readonly IAuthenticationService authenticationService;

        public MeCommand(IConsole console, IAuthenticationService authenticationService)
            : base(console)
        {
            this.authenticationService = authenticationService;
        }

        public override string Name => "/me";

        public async override Task<OptionalResult<string>> Execute(string token)
        {
            var userResult = await this.authenticationService.GetUserByToken(token);
            if (!userResult.IsSuccess)
            {
                return new OptionalResult<string>(userResult);
            }

            var user = userResult.Value;

            this.Output(user);

            return new OptionalResult<string>();
        }

        private void Output(UserModel user)
        {
            this.Console.Print("\n");
            this.Console.Print($"Username: {user.UserName}\n");
            this.Console.Print($"Email: {user.Email}\n");
            this.Console.Print("\n");
        }
    }
}
