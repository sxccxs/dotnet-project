using System.Diagnostics;
using BLL.Abstractions.Interfaces.UserInterfaces;
using Console.PrL.Interfaces;
using Core.DataClasses;
using Core.Models.UserModels;

namespace Console.PrL.Commands.UserCommands
{
    internal class EditUserInfoCommand : Command
    {
        private readonly IAuthenticationService authenticationService;
        private readonly IEditUserInfoService editUserInfoService;

        public EditUserInfoCommand(
            IConsole console,
            IAuthenticationService authenticationService,
            IEditUserInfoService editUserInfoService)
            : base(console)
        {
            this.authenticationService = authenticationService;
            this.editUserInfoService = editUserInfoService;
        }

        public override string Name => "/edituserinfo";

        public override string Description => "You need to log in to edit user information.";

        public override async Task<OptionalResult<string>> Execute(string token)
        {
            var userResult = await this.authenticationService.GetUserByToken(token);
            if (!userResult.IsSuccess)
            {
                return new OptionalResult<string>(userResult);
            }

            var user = userResult.Value;
            var userUpdateData = new OptionalResult<UserUpdateModel>();

            var infoToChange = this.GetInfoToChange();

            userUpdateData.Value.Id = user.Id.Value;
            this.SetNewParameter(infoToChange, ref userUpdateData);

            var editInfoResult = this.editUserInfoService.EditUser(userUpdateData, infoToChange);
            return new OptionalResult<string>();
        }

        private string GetInfoToChange()
        {
            var infoToChange =
                this.Console.Input("What would you like to change? \n - e-mail \n - password \n - username");
            while (true)
            {
                switch (infoToChange)
                {
                    case "e-mail": return "e-mail";
                    case "password": return "password";
                    case "username": return "username";
                    default:
                        this.Console.Print("Wrong input.");
                        break;
                }
            }
        }

        private void SetNewParameter(string infoToChange, ref OptionalResult<UserUpdateModel> userUpdateData)
        {
            var newInfo = this.Console.Input("Set a new value");

            switch (infoToChange)
            {
                case "e-mail": userUpdateData.Value.Email = newInfo;
                    break;
                case "username": userUpdateData.Value.UserName = newInfo;
                    break;
                case "password":
                    var repeat = this.Console.Input("repeat the new password");
                    if (repeat == newInfo)
                    {
                        userUpdateData.Value.Password = newInfo;
                    }

                    this.Console.Input("Wrong password.");
                    break;
                default: this.Console.Print("Wrong Input");
                    break;
            }
        }
    }
}
