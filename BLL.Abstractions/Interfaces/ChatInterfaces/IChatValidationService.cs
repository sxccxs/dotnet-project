using Core.DataClasses;
using Core.Models.ChatModels;

namespace BLL.Abstractions.Interfaces.ChatInterfaces;

public interface IChatValidationService
{
    ExceptionalResult ValidateCreateModel(ChatCreateModel createModel);

    ExceptionalResult ValidateEditModel(ChatEditModel editModel);
}