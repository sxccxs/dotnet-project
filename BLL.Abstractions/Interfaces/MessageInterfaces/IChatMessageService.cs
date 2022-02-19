using Core.DataClasses;
using Core.Models.ChatModels;
using Core.Models.MessageModels;
using Core.Models.UserModels;

namespace BLL.Abstractions.Interfaces.MessageInterfaces;

public interface IChatMessageService
{
    Task<ExceptionalResult> SendMessageToChatByUser(CreateMessageModel createModel);

    Task<ExceptionalResult> ReplyToMessageInChatByUser(CreateMessageModel replyMessage, MessageModel replyToMessage);

    Task<ExceptionalResult> ForwardMessageToChatByUser(MessageModel message, TextChatModel chat, UserModel user);

    Task<ExceptionalResult> EditMessageByUser(EditMessageModel editMessage, UserModel user);

    Task<ExceptionalResult> DeleteMessageByUser(MessageModel messageModel, UserModel user);
}
