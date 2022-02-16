using AutoMapper;
using BLL.Abstractions.Interfaces.AuditInterfaces;
using BLL.Abstractions.Interfaces.ChatInterfaces;
using BLL.Abstractions.Interfaces.RoleInterfaces;
using Core.DataClasses;
using Core.Enums;
using Core.Models.AuditModels;
using Core.Models.ChatModels;
using Core.Models.RoomModels;
using Core.Models.UserModels;
using DAL.Abstractions.Interfaces;

namespace BLL.Services.ChatServices;

public class RoomTextChatService : IRoomTextChatService
{
    private readonly IUserRoomRoleService roleService;

    private readonly ITextChatService chatService;

    private readonly IChatValidationService chatValidationService;

    private readonly IAuditService auditService;

    private readonly ITransactionsWorker transactionsWorker;

    public RoomTextChatService(
        ITransactionsWorker transactionsWorker,
        IAuditService auditService,
        IUserRoomRoleService roleService,
        ITextChatService chatService,
        IChatValidationService chatValidationService)
    {
        this.auditService = auditService;
        this.roleService = roleService;
        this.chatService = chatService;
        this.chatValidationService = chatValidationService;
        this.transactionsWorker = transactionsWorker;
    }

    public IEnumerable<TextChatModel> GetPublicTextChatsForUserAndRoom(UserModel user, RoomModel room)
    {
        return user.TextChats.Where(tc => tc.Room == room && !tc.IsPrivate);
    }

    public IEnumerable<TextChatModel> GetPrivateTextChatsForUserAndRoom(UserModel user, RoomModel room)
    {
        return user.TextChats.Where(tc => tc.Room == room && tc.IsPrivate);
    }

    public async Task<ExceptionalResult> CreatePublicTextChatInRoomByUser(RoomModel room, UserModel user, ChatCreateModel createModel, bool asTransaction = true)
    {
        return asTransaction
            ? await this.transactionsWorker.RunAsTransaction(() =>
                this.InnerCreatePublicTextChatInRoomByUser(room, user, createModel))
            : await this.InnerCreatePublicTextChatInRoomByUser(room, user, createModel);
    }

    public async Task<ExceptionalResult> CreatePrivateTextChatInRoomForUsers(RoomModel room, UserModel userCreator, UserModel user, ChatCreateModel createModel, bool asTransaction = true)
    {
        return asTransaction
            ? await this.transactionsWorker.RunAsTransaction(() =>
                this.InnerCreatePrivateTextChatInRoomForUsers(room, userCreator, user, createModel))
            : await this.InnerCreatePrivateTextChatInRoomForUsers(room, userCreator, user, createModel);
    }

    public async Task<ExceptionalResult> DeletePublicChatInRoomByUser(RoomModel room, UserModel user, TextChatModel textChat, bool asTransaction = true)
    {
        return asTransaction
            ? await this.transactionsWorker.RunAsTransaction(() =>
                this.DeleteChatInRoomForUser(room, user, textChat))
            : await this.DeleteChatInRoomForUser(room, user, textChat);
    }

    public async Task<ExceptionalResult> DeletePrivateChatInRoomByUser(RoomModel room, UserModel user, TextChatModel textChat, bool asTransaction = true)
    {
        return asTransaction
            ? await this.transactionsWorker.RunAsTransaction(() =>
                this.DeleteChatInRoomForUser(room, user, textChat, RoleType.Member))
            : await this.DeleteChatInRoomForUser(room, user, textChat, RoleType.Member);
    }

    public async Task<ExceptionalResult> UpdateTextChatInRoomByUser(RoomModel room, UserModel user, ChatEditModel editModel, bool asTransaction = true)
    {
        return asTransaction
            ? await this.transactionsWorker.RunAsTransaction(() =>
                this.InnerUpdateTextChatInRoomByUser(room, user, editModel))
            : await this.InnerUpdateTextChatInRoomByUser(room, user, editModel);
    }

    public async Task<ExceptionalResult> AddUserToPublicTextChatInRoomByUser(RoomModel room, UserModel userToAdd, UserModel user, TextChatModel textChatModel, bool asTransaction = true)
    {
        return asTransaction
            ? await this.transactionsWorker.RunAsTransaction(() =>
                this.InnerAddUserToPublicTextChatInRoomByUser(room, userToAdd, user, textChatModel))
            : await this.InnerAddUserToPublicTextChatInRoomByUser(room, userToAdd, user, textChatModel);
    }

    public async Task<ExceptionalResult> RemoveUserFromPublicTextChatInRoomByUser(RoomModel room, UserModel userToRemove, UserModel user, TextChatModel textChatModel, bool asTransaction = true)
    {
        return asTransaction
            ? await this.transactionsWorker.RunAsTransaction(() =>
                this.InnerRemoveUserFromPublicTextChatInRoomByUser(room, userToRemove, user, textChatModel))
            : await this.InnerRemoveUserFromPublicTextChatInRoomByUser(room, userToRemove, user, textChatModel);
    }

    public ExceptionalResult CheckUserInChat(UserModel user, TextChatModel chat)
    {
        return chat.Users.Contains(user)
            ? new ExceptionalResult()
            : new ExceptionalResult(false, $"User {user.Id} does not belong to chat{chat.Id}");
    }

    public async Task<ExceptionalResult> ValidateRoomUserChatRole(RoomModel room, UserModel user, TextChatModel chat, RoleType minRole = RoleType.Admin)
    {
        var result = await this.CheckRoomUserRole(room, user, minRole);

        return result.IsSuccess ? this.CheckUserInChat(user, chat) : result;
    }

    private async Task<ExceptionalResult> InnerUpdateTextChatInRoomByUser(RoomModel room, UserModel user, ChatEditModel editModel)
    {
        var chat = await this.chatService.GetTextChatById(editModel.Id);
        var roleResult = await this.ValidateRoomUserChatRole(room, user, chat);
        if (!roleResult.IsSuccess)
        {
            return roleResult;
        }

        var updateModel = this.MapEditModelToUpdateModel(editModel);

        var result = await this.chatService.Update(updateModel);
        if (!result.IsSuccess)
        {
            return result;
        }

        var record = new CreateAuditRecordModel()
        {
            ActionType = ActionType.EditTextChatInfo,
            Actor = user,
            Room = room,
            TextChat = chat,
        };

        return await this.auditService.CreateAuditRecord(record);
    }

    private async Task<ExceptionalResult> InnerAddUserToPublicTextChatInRoomByUser(RoomModel room, UserModel userToAdd, UserModel user, TextChatModel textChatModel)
    {
        var role1Result = await this.ValidateRoomUserChatRole(room, user, textChatModel);
        if (!role1Result.IsSuccess)
        {
            return role1Result;
        }

        var role2Result = await this.CheckRoomUserRole(room, userToAdd, RoleType.Member);
        if (!role2Result.IsSuccess)
        {
            return role2Result;
        }

        textChatModel.Users.Add(userToAdd);

        var result = await this.UpdateChatUsers(textChatModel);
        if (!result.IsSuccess)
        {
            return result;
        }

        var record = new CreateAuditRecordModel()
        {
            ActionType = ActionType.AddUserToTextChat,
            Actor = user,
            Room = room,
            UserUnderAction = userToAdd,
            TextChat = textChatModel,
        };

        return await this.auditService.CreateAuditRecord(record);
    }

    private async Task<ExceptionalResult> InnerRemoveUserFromPublicTextChatInRoomByUser(RoomModel room, UserModel userToRemove, UserModel user, TextChatModel textChatModel)
    {
        var role1Result = await this.ValidateRoomUserChatRole(room, user, textChatModel);
        if (!role1Result.IsSuccess)
        {
            return role1Result;
        }

        var role2Result = await this.ValidateRoomUserChatRole(room, userToRemove, textChatModel, RoleType.Member);
        if (!role2Result.IsSuccess)
        {
            return role2Result;
        }

        textChatModel.Users.Remove(userToRemove);

        var result = await this.UpdateChatUsers(textChatModel);
        if (!result.IsSuccess)
        {
            return result;
        }

        var record = new CreateAuditRecordModel()
        {
            ActionType = ActionType.DeleteUserFromTextChat,
            Actor = user,
            Room = room,
            UserUnderAction = userToRemove,
            TextChat = textChatModel,
        };

        return await this.auditService.CreateAuditRecord(record);
    }

    private async Task<ExceptionalResult> InnerCreatePublicTextChatInRoomByUser(RoomModel room, UserModel user, ChatCreateModel createModel)
    {
        createModel.IsPrivate = false;
        createModel.Users = new List<UserModel>() { user };

        var result = await this.CreateTextChatInRoomForUser(room, user, createModel);
        if (!result.IsSuccess)
        {
            return result;
        }

        var record = new CreateAuditRecordModel()
        {
            ActionType = ActionType.CreateTextChat,
            Actor = user,
            Room = room,
            TextChat = result.Value,
        };

        return await this.auditService.CreateAuditRecord(record);
    }

    private async Task<ExceptionalResult> InnerCreatePrivateTextChatInRoomForUsers(RoomModel room, UserModel userCreator, UserModel user, ChatCreateModel createModel)
    {
        createModel.IsPrivate = true;
        createModel.Users = new List<UserModel>() { userCreator, user };
        return await this.CreateTextChatInRoomForUser(room, userCreator, createModel, RoleType.Member);
    }

    private async Task<OptionalResult<TextChatModel>> CreateTextChatInRoomForUser(RoomModel room, UserModel user, ChatCreateModel createModel, RoleType minRole = RoleType.Admin)
    {
        var roleResult = await this.CheckRoomUserRole(room, user, minRole);
        if (!roleResult.IsSuccess)
        {
            return new OptionalResult<TextChatModel>(roleResult);
        }

        var validationResult = this.chatValidationService.ValidateCreateModel(createModel);
        if (!validationResult.IsSuccess)
        {
            return new OptionalResult<TextChatModel>(validationResult);
        }

        createModel.Room = room;

        return await this.chatService.Create(createModel);
    }

    private async Task<ExceptionalResult> DeleteChatInRoomForUser(RoomModel room, UserModel user, TextChatModel textChat, RoleType minRole = RoleType.Admin)
    {
        var roleResult = await this.ValidateRoomUserChatRole(room, user, textChat, minRole);
        if (!roleResult.IsSuccess)
        {
            return roleResult;
        }

        var result = await this.chatService.Delete(textChat.Id);
        if (!result.IsSuccess || textChat.IsPrivate)
        {
            return result;
        }

        var record = new CreateAuditRecordModel()
        {
            ActionType = ActionType.DeleteTextChat,
            Actor = user,
            Room = room,
            TextChat = textChat,
        };

        return await this.auditService.CreateAuditRecord(record);
    }

    private async Task<OptionalResult<TextChatModel>> UpdateChatUsers(TextChatModel chat)
    {
        var updateModel = new TextChatUpdateModel()
        {
            Id = chat.Id,
            Users = chat.Users,
        };

        return await this.chatService.Update(updateModel);
    }

    private async Task<ExceptionalResult> CheckRoomUserRole(RoomModel room, UserModel user, RoleType minRole = RoleType.Admin)
    {
        var role = await this.roleService.GetRoleForUserAndRoom(user, room);
        if (role is null || role.RoleType.Name != minRole.ToString())
        {
            return new ExceptionalResult(false, $"User {user.Id} does not belong to room {room.Id} or does not have rights for this operation");
        }

        return new ExceptionalResult();
    }

    private TextChatUpdateModel MapEditModelToUpdateModel(ChatEditModel editModel)
    {
        var mapperConfiguration = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ChatEditModel, TextChatUpdateModel>().ForAllMembers(opt => opt.AllowNull());
        });
        var mapper = new Mapper(mapperConfiguration);
        return mapper.Map<TextChatUpdateModel>(editModel);
    }
}