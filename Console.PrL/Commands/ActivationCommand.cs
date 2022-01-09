﻿using BLL.Abstractions.Interfaces;
using Console.PrL.Interfaces;
using Core.DataClasses;

namespace Console.PrL.Commands
{
    internal class ActivationCommand : Command
    {
        private readonly IAccountActivationService accountActivationService;

        public ActivationCommand(IConsole console, IAccountActivationService accountActivationService)
            : base(console)
        {
            this.accountActivationService = accountActivationService;
        }

        public override string Name => "/activate";

        public override OptionalResult<string> Execute()
        {
            var payload = this.GetActivationInfo();

            this.accountActivationService.Activate(payload);

            this.Console.Print("Your account has been successfully actived. You can login now.\n");

            return new OptionalResult<string>();
        }

        private AccountActivationPayload GetActivationInfo()
        {
            this.Console.Print("\n");
            var payload = new AccountActivationPayload()
            {
                Uidb64 = this.Console.Input("Uidb64: "),
                Token = this.Console.Input("Token: "),
            };
            this.Console.Print("\n");

            return payload;
        }
    }
}
