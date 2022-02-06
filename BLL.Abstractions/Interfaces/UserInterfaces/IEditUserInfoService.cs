using Core.DataClasses;
using Core.Models.UserModels;

namespace BLL.Abstractions.Interfaces.UserInterfaces
{
    public interface IEditUserInfoService
    {
        Task<ExceptionalResult> EditUser(UserEditModel editModel);

        bool CheckOldPassword(UserModel user, string oldPassword);
    }
}
