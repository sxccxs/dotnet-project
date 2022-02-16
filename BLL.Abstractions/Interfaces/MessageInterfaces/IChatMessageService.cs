using Core.DataClasses;
using Core.Models.ChatModels;
using Core.Models.MessageModels;
using Core.Models.UserModels;

namespace BLL.Abstractions.Interfaces.MessageInterfaces;

public interface IChatMessageService
{
    Task<ExceptionalResult> SendMessageToChatByUser(CreateMessageModel createModel, bool asTransaction);

    Task<ExceptionalResult> ReplyToMessageInChatByUser(CreateMessageModel replyMessage, MessageModel replyToMessage, bool asTransaction);

    Task<ExceptionalResult> ForwardMessageToChatByUser(MessageModel message, TextChatModel chat, UserModel user, bool asTransaction);

    Task<ExceptionalResult> EditMessageByUser(EditMessageModel editMessage, UserModel user, bool asTransaction);

    Task<ExceptionalResult> DeleteMessageByUser(MessageModel messageModel, UserModel user, bool asTransaction);
}