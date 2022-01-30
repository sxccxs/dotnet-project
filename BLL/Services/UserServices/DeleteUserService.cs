using BLL.Abstractions.Interfaces.UserInterfaces;
using Core.DataClasses;
using Core.Models.UserModels;

namespace BLL.Services.UserServices
{
    internal class DeleteUserService : IDeleteUserService
    {
        private readonly IUserService userService;

        public DeleteUserService(IUserService userService)
        {
            this.userService = userService;
        }

        public async Task<ExceptionalResult> DeleteUser(UserModel user)
        {
            await this.userService.Delete(user.Id);
            return new ExceptionalResult();
        }
    }
}
