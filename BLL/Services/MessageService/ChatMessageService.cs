using AutoMapper;
using BLL.Abstractions.Interfaces.AuditInterfaces;
using BLL.Abstractions.Interfaces.ChatInterfaces;
using BLL.Abstractions.Interfaces.MessageInterfaces;
using Core.DataClasses;
using Core.Enums;
using Core.Models.AuditModels;
using Core.Models.ChatModels;
using Core.Models.MessageModels;
using Core.Models.UserModels;
using DAL.Abstractions.Interfaces;

namespace BLL.Services.MessageService;

public class ChatMessageService : IChatMessageService
{
    private readonly ITransactionsWorker transactionsWorker;

    private readonly IMessageService messageService;

    private readonly ITextChatService chatService;

    private readonly IRoomTextChatService roomChatService;

    private readonly IAuditService auditService;

    public ChatMessageService(
        ITransactionsWorker transactionsWorker,
        IRoomTextChatService roomChatService,
        IMessageService messageService,
        IAuditService auditService,
        ITextChatService chatService)
    {
        this.auditService = auditService;
        this.transactionsWorker = transactionsWorker;
        this.chatService = chatService;
        this.messageService = messageService;
        this.roomChatService = roomChatService;
    }

    public async Task<ExceptionalResult> SendMessageToChatByUser(CreateMessageModel createModel)
    {
        var message = this.MapCreateModelToMessageModel(createModel);

        return await this.transactionsWorker.RunAsTransaction(() => this.CreateMessageForUserAndChat(message));
    }

    public async Task<ExceptionalResult> ReplyToMessageInChatByUser(CreateMessageModel replyMessage, MessageModel replyToMessage)
    {
        var message = this.MapCreateModelToMessageModel(replyMessage);
        message.ReplyTo = replyToMessage;

        return await this.transactionsWorker.RunAsTransaction(() => this.CreateMessageForUserAndChat(message));
    }

    public async Task<ExceptionalResult> ForwardMessageToChatByUser(MessageModel forwardMessage, TextChatModel chat, UserModel user)
    {
        var message = new MessageModel()
        {
            Author = user,
            Chat = chat,
            Text = forwardMessage.Text,
            ForwardedFrom = forwardMessage.ForwardedFrom ?? forwardMessage.Author,
        };

        return await this.transactionsWorker.RunAsTransaction(() => this.CreateMessageForUserAndChat(message));
    }

    public async Task<ExceptionalResult> EditMessageByUser(EditMessageModel editMessage, UserModel user)
    {
        return await this.transactionsWorker.RunAsTransaction(() => this.InnerEditMessageByUser(editMessage, user));
    }

    public async Task<ExceptionalResult> DeleteMessageByUser(MessageModel messageModel, UserModel user)
    {
        return await this.transactionsWorker.RunAsTransaction(() => this.InnerDeleteMessageByUser(messageModel, user));
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

        var creationResult = await this.messageService.Create(messageModel);
        if (!creationResult.IsSuccess)
        {
            return creationResult;
        }

        return await this.CreateAuditRecordForMessage(messageModel);
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

    private async Task<ExceptionalResult> CreateAuditRecordForMessage(MessageModel message)
    {
        CreateAuditRecordModel createModel = null;
        if (message.ForwardedFrom is not null)
        {
            createModel = new CreateAuditRecordModel()
            {
                ActionType = ActionType.MessageForward,
                Actor = message.Author,
                Room = message.Chat.Room,
                TextChat = message.Chat,
                UserUnderAction = message.ForwardedFrom,
            };

            return await this.auditService.CreateAuditRecord(createModel);
        }

        if (message.ReplyTo is not null)
        {
            createModel = new CreateAuditRecordModel()
            {
                ActionType = ActionType.MessageReply,
                Actor = message.Author,
                Room = message.Chat.Room,
                TextChat = message.Chat,
                UserUnderAction = message.ReplyTo.Author,
            };
        }

        return createModel is not null
            ? await this.auditService.CreateAuditRecord(createModel)
            : new ExceptionalResult();
    }
}
