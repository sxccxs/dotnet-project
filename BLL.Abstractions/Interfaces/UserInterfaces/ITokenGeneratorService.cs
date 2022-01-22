using Core.DataClasses;
using Core.Models.UserModels;

namespace BLL.Abstractions.Interfaces.UserInterfaces
{
    public interface ITokenGeneratorService
    {
        ExceptionalResult CheckToken(UserModel user, string token);

        OptionalResult<int> GetIdFromUidb64(string uidb64);

        string GetToken(UserModel user);

        string GetUidb64(UserModel user);
    }
}
