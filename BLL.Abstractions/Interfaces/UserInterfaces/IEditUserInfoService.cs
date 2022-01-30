using Core.DataClasses;
using Core.Models.UserModels;

namespace BLL.Abstractions.Interfaces.UserInterfaces
{
    public interface IEditUserInfoService
    {
        Task<OptionalResult<string>> EditUser(OptionalResult<UserUpdateModel> user, string updatedInfo);
    }
}
