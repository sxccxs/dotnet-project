using Core.Models;

namespace BLL.Abstractions.Interfaces
{
    public interface ITokenGeneratorService
    {
        void CheckToken(UserModel user, string token);

        int GetIdFromUidb64(string uidb64);

        string GetToken(UserModel user);

        string GetUidb64(UserModel user);
    }
}
