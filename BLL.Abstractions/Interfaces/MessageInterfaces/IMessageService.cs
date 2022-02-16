using System.Linq.Expressions;
using Core.DataClasses;
using Core.Models.MessagesModels;

namespace BLL.Abstractions.Interfaces.MessageInterfaces;

public interface IMessageService
{
    Task<IEnumerable<MessageModel>> GetByConditions(params Expression<Func<MessageModel, bool>>[] conditions);

    Task<MessageModel> GetRoomById(int id);

    Task<OptionalResult<MessageModel>> Create(MessageModel messageModel);

    Task<OptionalResult<MessageModel>> Update(MessageModel messageModel);

    Task<ExceptionalResult> Delete(int id);
}