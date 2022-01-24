﻿using BLL.Abstractions.Interfaces.UserInterfaces;
using Console.PrL.Interfaces;
using Core.DataClasses;
using Core.Models.UserModels;

namespace Console.PrL.Commands.UserCommands
{
    internal class RegistrationCommand : Command
    {
        private readonly IRegistrationService registrationService;

        public RegistrationCommand(IConsole console, IRegistrationService registrationService)
            : base(console)
        {
            this.registrationService = registrationService;
        }

        public override string Name => "/register";

        public override string Description => "Register in app";

        public async override Task<OptionalResult<string>> Execute(string token)
        {
            var userRegistration = this.GetRegistrationInfo();

            var registrationResult = await this.registrationService.Register(userRegistration);
            if (!registrationResult.IsSuccess)
            {
                return new OptionalResult<string>(registrationResult);
            }

            this.Console.Print("You registered successfully. Account activation email was sent to your address\n");

            return new OptionalResult<string>();
        }

        private UserRegistrationModel GetRegistrationInfo()
        {
            this.Console.Print("\n");
            var registrationModel = new UserRegistrationModel()
            {
                UserName = this.Console.Input("Username: "),
                Email = this.Console.Input("Email: "),
                Password = this.Console.Input("Password: "),
                RePassword = this.Console.Input("Repeat password: "),
            };
            this.Console.Print("\n");

            return registrationModel;
        }
    }
}
