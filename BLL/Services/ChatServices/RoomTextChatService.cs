using AutoMapper;
using BLL.Abstractions.Interfaces.ChatInterfaces;
using BLL.Abstractions.Interfaces.RoleInterfaces;
using BLL.Abstractions.Interfaces.RoomInterfaces;
using BLL.Abstractions.Interfaces.UserInterfaces;
using Core.DataClasses;
using Core.Enums;
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

    private readonly IUserService userService;

    private readonly IRoomService roomService;

    private readonly ITransactionsWorker transactionsWorker;

    public RoomTextChatService(
        ITransactionsWorker transactionsWorker,
        IUserRoomRoleService roleService,
        ITextChatService chatService,
        IUserService userService,
        IRoomService roomService,
        IChatValidationService chatValidationService)
    {
        this.roleService = roleService;
        this.chatService = chatService;
        this.userService = userService;
        this.roomService = roomService;
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
        createModel.IsPrivate = false;
        return asTransaction
            ? await this.transactionsWorker.RunAsTransaction(() =>
                this.CreateTextChatInRoomForUser(room, user, createModel))
            : await this.CreateTextChatInRoomForUser(room, user, createModel);
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
                this.DeleteChatInRoomForUser(room, user, textChat, Role.MEMBER))
            : await this.DeleteChatInRoomForUser(room, user, textChat, Role.MEMBER);
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

    private async Task<ExceptionalResult> InnerUpdateTextChatInRoomByUser(RoomModel room, UserModel user, ChatEditModel editModel)
    {
        var chat = await this.chatService.GetTextChatById(editModel.Id);
        var roleResult = await this.ValidateRoomUserChatRole(room, user, chat);
        if (!roleResult.IsSuccess)
        {
            return roleResult;
        }

        var updateModel = this.MapEditModelToUpdateModel(editModel);

        return await this.chatService.Update(updateModel);
    }

    private async Task<ExceptionalResult> InnerAddUserToPublicTextChatInRoomByUser(RoomModel room, UserModel userToAdd, UserModel user, TextChatModel textChatModel)
    {
        var role1Result = await this.ValidateRoomUserChatRole(room, user, textChatModel);
        if (!role1Result.IsSuccess)
        {
            return role1Result;
        }

        var role2Result = await this.CheckRoomUserRole(room, userToAdd, Role.MEMBER);
        if (!role2Result.IsSuccess)
        {
            return role2Result;
        }

        textChatModel.Users.Add(userToAdd);
        var updateResult = await this.UpdateChatUsers(textChatModel);

        return !updateResult.IsSuccess ? updateResult : new ExceptionalResult();
    }

    private async Task<ExceptionalResult> InnerRemoveUserFromPublicTextChatInRoomByUser(RoomModel room, UserModel userToRemove, UserModel user, TextChatModel textChatModel)
    {
        var role1Result = await this.ValidateRoomUserChatRole(room, user, textChatModel);
        if (!role1Result.IsSuccess)
        {
            return role1Result;
        }

        var role2Result = await this.ValidateRoomUserChatRole(room, userToRemove, textChatModel, Role.MEMBER);
        if (!role2Result.IsSuccess)
        {
            return role2Result;
        }

        textChatModel.Users.Remove(userToRemove);
        var updateResult = await this.UpdateChatUsers(textChatModel);

        return !updateResult.IsSuccess ? updateResult : new ExceptionalResult();
    }

    private async Task<ExceptionalResult> InnerCreatePrivateTextChatInRoomForUsers(RoomModel room, UserModel userCreator, UserModel user, ChatCreateModel createModel)
    {
        createModel.IsPrivate = true;
        var result = await this.CreateTextChatInRoomForUser(room, userCreator, createModel, Role.MEMBER);
        if (!result.IsSuccess)
        {
            return result;
        }

        user.TextChats.Add(result.Value);

        return await this.UpdateUserChats(user);
    }

    private async Task<OptionalResult<TextChatModel>> CreateTextChatInRoomForUser(RoomModel room, UserModel user, ChatCreateModel createModel, Role minRole = Role.ADMIN)
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

        var chatResult = await this.chatService.Create(createModel);
        if (!chatResult.IsSuccess)
        {
            return chatResult;
        }

        var chat = chatResult.Value;
        user.TextChats.Add(chat);
        var userResult = await this.UpdateUserChats(user);
        if (!userResult.IsSuccess)
        {
            return new OptionalResult<TextChatModel>(userResult);
        }

        room.TextChats.Add(chat);
        var roomResult = await this.UpdateRoomChats(room);

        return !roomResult.IsSuccess ? new OptionalResult<TextChatModel>(roomResult) : new OptionalResult<TextChatModel>(chat);
    }

    private async Task<ExceptionalResult> DeleteChatInRoomForUser(RoomModel room, UserModel user, TextChatModel textChat, Role minRole = Role.ADMIN)
    {
        var roleResult = await this.ValidateRoomUserChatRole(room, user, textChat, minRole);
        if (!roleResult.IsSuccess)
        {
            return roleResult;
        }

        room.TextChats.Remove(textChat);
        var roomResult = await this.UpdateRoomChats(room);
        if (!roomResult.IsSuccess)
        {
            return roomResult;
        }

        foreach (var chatUser in textChat.Users)
        {
            chatUser.TextChats.Remove(textChat);
            var userResult = await this.UpdateUserChats(chatUser);
            if (!userResult.IsSuccess)
            {
                return userResult;
            }
        }

        return await this.chatService.Delete(textChat.Id);
    }

    private async Task<OptionalResult<UserModel>> UpdateUserChats(UserModel user)
    {
        var updateModel = new UserUpdateModel()
        {
            Id = user.Id,
            TextChats = user.TextChats,
            IsActive = user.IsActive,
        };

        return await this.userService.Update(updateModel);
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

    private async Task<OptionalResult<RoomModel>> UpdateRoomChats(RoomModel room)
    {
        var updateModel = new RoomUpdateModel()
        {
            Id = room.Id,
            TextChats = room.TextChats,
        };

        return await this.roomService.Update(updateModel);
    }

    private async Task<ExceptionalResult> CheckRoomUserRole(RoomModel room, UserModel user, Role minRole = Role.ADMIN)
    {
        var role = await this.roleService.GetRoleForUserAndRoom(user, room);
        if (role is null || role.RoleType.Name != minRole.ToString())
        {
            return new ExceptionalResult(false, $"User {user.Id} does not belong to room {room.Id} or does not have rights for this operation");
        }

        return new ExceptionalResult();
    }

    private ExceptionalResult CheckUserInChat(UserModel user, TextChatModel chat)
    {
        return chat.Users.Contains(user)
            ? new ExceptionalResult()
            : new ExceptionalResult(false, $"User {user.Id} does not belong to chat{chat.Id}");
    }

    private async Task<ExceptionalResult> ValidateRoomUserChatRole(RoomModel room, UserModel user, TextChatModel chat, Role minRole = Role.ADMIN)
    {
        var result = await this.CheckRoomUserRole(room, user, minRole);

        return result.IsSuccess ? this.CheckUserInChat(user, chat) : result;
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