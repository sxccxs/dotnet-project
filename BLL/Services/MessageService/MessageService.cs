using System.Linq.Expressions;
using BLL.Abstractions.Interfaces.MessageInterfaces;
using Core.DataClasses;
using Core.Models.MessagesModels;
using DAL.Abstractions.Interfaces;

namespace BLL.Services.MessageService;

public class MessageService : IMessageService
{
    private readonly IGenericStorageWorker<MessageModel> storage;

    public MessageService(IGenericStorageWorker<MessageModel> storage)
    {
        this.storage = storage;
    }

    public async Task<IEnumerable<MessageModel>> GetByConditions(
        params Expression<Func<MessageModel, bool>>[] conditions)
    {
        return await this.storage.GetByConditions(
            conditions,
            m => m.Author,
            m => m.Chat,
            m => m.ForwardedFrom,
            m => m.ReplyTo);
    }

    public async Task<MessageModel> GetMessageById(int id)
    {
        return (await this.GetByConditions(m => m.Id == id)).FirstOrDefault();
    }

    public async Task<OptionalResult<MessageModel>> Create(MessageModel messageModel)
    {
        await this.storage.Create(messageModel);

        return new OptionalResult<MessageModel>(messageModel);
    }

    public async Task<OptionalResult<MessageModel>> Update(MessageModel messageModel)
    {
        if (await this.GetMessageById(messageModel.Id) is null)
        {
            return new OptionalResult<MessageModel>(false, $"Message with id {messageModel.Id} does not exist");
        }

        messageModel.IsEdited = true;
        await this.storage.Update(messageModel);

        return new OptionalResult<MessageModel>(messageModel);
    }

    public async Task<ExceptionalResult> Delete(int id)
    {
        var message = await this.GetMessageById(id);
        if (message is null)
        {
            return new ExceptionalResult(false, $"Message with id {id} does not exist");
        }

        await this.storage.Delete(message);

        return new ExceptionalResult();
    }
}