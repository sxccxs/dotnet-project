using System.Linq.Expressions;
using Core.DataClasses;
using Core.Models.ChatModels;

namespace BLL.Abstractions.Interfaces.ChatInterfaces;

public interface IVoiceChatService
{
    Task<IEnumerable<VoiceChatModel>> GetByConditions(params Expression<Func<VoiceChatModel, bool>>[] conditions);

    Task<VoiceChatModel> GetTextChatById(int id);

    Task<OptionalResult<VoiceChatModel>> Create(ChatCreateModel chatModel);

    Task<OptionalResult<VoiceChatModel>> Update(ChatUpdateModel chatModel);

    Task<ExceptionalResult> Delete(int id);
}