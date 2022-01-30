using Core.DataClasses;

namespace BLL.Abstractions.Interfaces.UserInterfaces;

public interface IUpdateValidationService
{
    ExceptionalResult ValidateEmail(string email);

    ExceptionalResult ValidatePassword(string password);

    ExceptionalResult ValidateUserName(string username);
}