using AutoMapper;
using BLL.Abstractions.Interfaces.ChatInterfaces;
using BLL.Abstractions.Interfaces.MessageInterfaces;
using BLL.Abstractions.Interfaces.UserInterfaces;
using Core.DataClasses;
using Core.Models.ChatModels;
using Core.Models.MessagesModels;
using Core.Models.UserModels;
using DAL.Abstractions.Interfaces;

namespace BLL.Services.MessageService;

public class ChatMessageService : IChatMessageService
{
    private readonly ITransactionsWorker transactionsWorker;

    private readonly IMessageService messageService;

    private readonly IUserService userService;

    private readonly ITextChatService chatService;

    private readonly IRoomTextChatService roomChatService;

    public ChatMessageService(ITransactionsWorker transactionsWorker, IRoomTextChatService roomChatService, IMessageService messageService, IUserService userService, ITextChatService chatService)
    {
        this.transactionsWorker = transactionsWorker;
        this.chatService = chatService;
        this.messageService = messageService;
        this.userService = userService;
        this.roomChatService = roomChatService;
    }

    public async Task<ExceptionalResult> SendMessageToChatByUser(CreateMessageModel createModel, bool asTransaction)
    {
        var message = this.MapCreateModelToMessageModel(createModel);

        return asTransaction
            ? await this.transactionsWorker.RunAsTransaction(() => this.CreateMessageForUserAndChat(message))
            : await this.CreateMessageForUserAndChat(message);
    }

    public async Task<ExceptionalResult> ReplyToMessageInChatByUser(CreateMessageModel replyMessage, MessageModel replyToMessage, bool asTransaction)
    {
        var message = this.MapCreateModelToMessageModel(replyMessage);
        message.ReplyTo = replyToMessage;

        return asTransaction
            ? await this.transactionsWorker.RunAsTransaction(() => this.CreateMessageForUserAndChat(message))
            : await this.CreateMessageForUserAndChat(message);
    }

    public async Task<ExceptionalResult> ForwardMessageToChatByUser(MessageModel forwardMessage, TextChatModel chat, UserModel user, bool asTransaction)
    {
        var message = new MessageModel()
        {
            Author = user,
            Chat = chat,
            Text = forwardMessage.Text,
            ForwardedFrom = forwardMessage.ForwardedFrom ?? forwardMessage.Author,
        };

        return asTransaction
            ? await this.transactionsWorker.RunAsTransaction(() => this.CreateMessageForUserAndChat(message))
            : await this.CreateMessageForUserAndChat(message);
    }

    public async Task<ExceptionalResult> EditMessageByUser(EditMessageModel editMessage, UserModel user, bool asTransaction)
    {
        return asTransaction
            ? await this.transactionsWorker.RunAsTransaction(() => this.InnerEditMessageByUser(editMessage, user))
            : await this.InnerEditMessageByUser(editMessage, user);
    }

    public async Task<ExceptionalResult> DeleteMessageByUser(MessageModel messageModel, UserModel user, bool asTransaction)
    {
        return asTransaction
            ? await this.transactionsWorker.RunAsTransaction(() => this.InnerDeleteMessageByUser(messageModel, user))
            : await this.InnerDeleteMessageByUser(messageModel, user);
    }

    private async Task<ExceptionalResult> CreateMessageForUserAndChat(MessageModel messageModel)
    {
        var user = messageModel.Author;
        var chat = messageModel.Chat;
        var validationResult = this.roomChatService.CheckUserInChat(user, chat);
        if (!validationResult.IsSuccess)
        {
            return validationResult;
        }

        var messageResult = await this.messageService.Create(messageModel);
        if (!messageResult.IsSuccess)
        {
            return messageResult;
        }

        user.Messages.Add(messageModel);
        var userResult = await this.UpdateUserMessages(user);
        if (!userResult.IsSuccess)
        {
            return userResult;
        }

        chat.Messages.Add(messageModel);

        return await this.UpdateChatMessages(chat);
    }

    private async Task<ExceptionalResult> InnerEditMessageByUser(EditMessageModel editMessage, UserModel user)
    {
        var messageResult = await this.ValidateMessageAndUser(editMessage.Id, user);
        if (!messageResult.IsSuccess)
        {
            return messageResult;
        }

        var message = messageResult.Value;
        message.Text = editMessage.Text;
        message.IsEdited = true;

        return await this.messageService.Update(message);
    }

    private async Task<ExceptionalResult> InnerDeleteMessageByUser(MessageModel messageModel, UserModel user)
    {
        var messageResult = await this.ValidateMessageAndUser(messageModel.Id, user);
        if (!messageResult.IsSuccess)
        {
            return messageResult;
        }

        var message = messageResult.Value;
        return await this.messageService.Delete(message.Id);
    }

    private async Task<OptionalResult<MessageModel>> ValidateMessageAndUser(int messageId, UserModel user)
    {
        var message = await this.messageService.GetMessageById(messageId);
        if (message is null)
        {
            return new OptionalResult<MessageModel>(false, $"Message with id {messageId} does not exist");
        }

        var chat = await this.chatService.GetTextChatById(message.Chat.Id);
        var validationResult = await this.roomChatService.ValidateRoomUserChatRole(chat.Room, user, chat);
        if (!validationResult.IsSuccess && message.Author != user)
        {
            return new OptionalResult<MessageModel>(validationResult);
        }

        return new OptionalResult<MessageModel>(message);
    }

    private MessageModel MapCreateModelToMessageModel(CreateMessageModel createModel)
    {
        var mapperConfiguration = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CreateMessageModel, MessageModel>(MemberList.Source);
        });
        var mapper = new Mapper(mapperConfiguration);

        return mapper.Map<MessageModel>(createModel);
    }

    private async Task<ExceptionalResult> UpdateUserMessages(UserModel user)
    {
        var updateModel = new UserUpdateModel()
        {
            Id = user.Id,
            Messages = user.Messages,
        };

        return await this.userService.Update(updateModel);
    }

    private async Task<ExceptionalResult> UpdateChatMessages(TextChatModel chat)
    {
        var updateModel = new TextChatUpdateModel()
        {
            Id = chat.Id,
            Messages = chat.Messages,
        };

        return await this.chatService.Update(updateModel);
    }
}