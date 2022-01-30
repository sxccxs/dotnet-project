using BLL.Abstractions.Interfaces.UserInterfaces;
using Console.PrL.Interfaces;
using Core.DataClasses;
using Core.Models.UserModels;

namespace Console.PrL.Commands.UserCommands
{
    internal class EditAccountCommand : Command
    {
        private readonly IAuthenticationService authenticationService;

        private readonly IEditUserInfoService editService;

        public EditAccountCommand(IConsole console, IAuthenticationService authenticationService, IEditUserInfoService editService)
            : base(console)
        {
            this.authenticationService = authenticationService;
            this.editService = editService;
        }

        public override string Name => "/editAccount";

        public override string Description => "Edits your account info";

        public override async Task<OptionalResult<string>> Execute(string token)
        {
            var userResult = await this.authenticationService.GetUserByToken(token);
            if (!userResult.IsSuccess)
            {
                return new OptionalResult<string>(userResult);
            }

            var user = userResult.Value;
            var editData = this.GetEditData(user);
            var result = await this.editService.EditUser(editData);
            if (!result.IsSuccess)
            {
                return new OptionalResult<string>(result);
            }

            this.Console.Print("Updated successfully\n");

            return new OptionalResult<string>();
        }

        private UserEditModel GetEditData(UserModel user)
        {
            var editModel = new UserEditModel()
            {
                Id = user.Id,
            };
            var userName = this.Console.Input("Enter new username(leave blank to leave previous): ");
            if (!string.IsNullOrWhiteSpace(userName))
            {
                editModel.UserName = userName;
            }

            var email = this.Console.Input("Enter new email(leave blank to leave previous): ");
            if (!string.IsNullOrWhiteSpace(email))
            {
                this.Console.Print("You will have to confirm your email again. Activation email will be sent to you\n");
                editModel.Email = email;
            }

            var password = this.Console.Input("Enter new password(leave blank to leave previous): ");
            if (!string.IsNullOrWhiteSpace(password))
            {
                if (!this.CheckOldPassword(user))
                {
                    this.Console.Print("Old password is incorrect\n");
                }
                else
                {
                    editModel.Password = password;
                }
            }

            return editModel;
        }

        private bool CheckOldPassword(UserModel user)
        {
            var oldPassword = this.Console.Input("Enter old password: ");
            return this.editService.CheckOldPassword(user, oldPassword);
        }
    }
}
