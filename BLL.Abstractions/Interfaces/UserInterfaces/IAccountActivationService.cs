using Core.DataClasses;

namespace BLL.Abstractions.Interfaces.UserInterfaces
{
    public interface IAccountActivationService
    {
        ExceptionalResult Activate(AccountActivationPayload activationPayload);
    }
}
