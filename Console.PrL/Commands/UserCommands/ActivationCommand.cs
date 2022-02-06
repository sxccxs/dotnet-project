using BLL.Abstractions.Interfaces.UserInterfaces;
using Console.PrL.Interfaces;
using Core.DataClasses;

namespace Console.PrL.Commands.UserCommands
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

        public override string Description => "Activate your account after registration";

        public async override Task<OptionalResult<string>> Execute(string token)
        {
            var payload = this.GetActivationInfo();

            var activationResult = await this.accountActivationService.Activate(payload);
            if (!activationResult.IsSuccess)
            {
                return new OptionalResult<string>(activationResult);
            }

            this.Console.Print("Your account has been successfully actived. You can login now.");

            return new OptionalResult<string>();
        }

        private AccountActivationPayload GetActivationInfo()
        {
            this.Console.Print();
            var payload = new AccountActivationPayload()
            {
                Uidb64 = this.Console.Input("Uidb64: "),
                Token = this.Console.Input("Token: "),
            };
            this.Console.Print();

            return payload;
        }
    }
}
