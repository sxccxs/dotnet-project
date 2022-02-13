using System.Linq.Expressions;
using Core.DataClasses;
using Core.Models.ChatModels;

namespace BLL.Abstractions.Interfaces.ChatInterfaces;

public interface ITextChatService
{
    Task<IEnumerable<TextChatModel>> GetByConditions(params Expression<Func<TextChatModel, bool>>[] conditions);

    Task<TextChatModel> GetTextChatById(int id);

    Task<OptionalResult<TextChatModel>> Create(ChatCreateModel chatModel);

    Task<OptionalResult<TextChatModel>> Update(TextChatUpdateModel chatModel);

    Task<ExceptionalResult> Delete(int id);
}