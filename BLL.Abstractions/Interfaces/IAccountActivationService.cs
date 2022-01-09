using Core.DataClasses;

namespace BLL.Abstractions.Interfaces
{
    public interface IAccountActivationService
    {
        void Activate(AccountActivationPayload activationPayload);
    }
}
