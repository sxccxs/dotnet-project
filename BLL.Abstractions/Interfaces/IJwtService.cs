using Core.DataClasses;
using Core.Models;

namespace BLL.Abstractions.Interfaces
{
    public interface IJwtService
    {
        string GenerateJwt(UserModel user);

        OptionalResult<int> ValidateJwt(string jwt);
    }
}
