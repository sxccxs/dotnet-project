using Core.DataClasses;

namespace BLL.Abstractions.Interfaces.UserInterfaces
{
    public interface IAccountActivationService
    {
        Task<ExceptionalResult> Activate(AccountActivationPayload activationPayload);
    }
}
