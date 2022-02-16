using BLL.Abstractions.Interfaces.ChatInterfaces;
using Core.DataClasses;
using Core.Models.ChatModels;

namespace BLL.Services.ChatServices;

public class ChatValidationService : IChatValidationService
{
    private const int MaxChatNameLength = 32;

    public ExceptionalResult ValidateCreateModel(ChatCreateModel createModel)
    {
        var results = new ExceptionalResult[]
        {
            this.ValidateChatName(createModel.Name),
        };

        var incorrectResults = results.Where(r => !r.IsSuccess).ToList();

        return incorrectResults.Any() ? incorrectResults.First() : new ExceptionalResult();
    }

    public ExceptionalResult ValidateEditModel(ChatEditModel editModel)
    {
        var results = new ExceptionalResult[]
        {
            this.ValidateChatName(editModel.Name),
        };

        var incorrectResults = results.Where(r => !r.IsSuccess).ToList();

        return incorrectResults.Any() ? incorrectResults.First() : new ExceptionalResult();
    }

    private ExceptionalResult ValidateChatName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return new ExceptionalResult(false, "Chat name can't be empty");
        }

        if (name.Length > MaxChatNameLength)
        {
            return new ExceptionalResult(false, $"Chat name can't be longer then {MaxChatNameLength} symbols");
        }

        return new ExceptionalResult();
    }
}