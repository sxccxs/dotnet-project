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

        public async Task<OptionalResult<string>> DeleteUser(OptionalResult<int> userId)
        {
            var user = (await this.userService.GetByCondition(x => x.Id == userId)).FirstOrDefault();
            if (user.Id.IsSuccess)
            {
                await this.userService.Delete(user.Id.Value);
                return new OptionalResult<string>();
            }

            return new OptionalResult<string>(false, "user id is wrong");
        }
    }
}
