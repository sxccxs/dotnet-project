using Core.DataClasses;

namespace BLL.Abstractions.Interfaces.UserInterfaces
{
    public interface IAccountActivationService
    {
        void Activate(AccountActivationPayload activationPayload);
    }
}
