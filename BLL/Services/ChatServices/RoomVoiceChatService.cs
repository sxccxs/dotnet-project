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

public class RoomVoiceChatService : IRoomVoiceChatService
{
    private readonly IUserRoomRoleService roleService;

    private readonly IVoiceChatService chatService;

    private readonly IChatValidationService chatValidationService;

    private readonly IUserService userService;

    private readonly IRoomService roomService;

    private readonly ITransactionsWorker transactionsWorker;

    public RoomVoiceChatService(
        ITransactionsWorker transactionsWorker,
        IUserRoomRoleService roleService,
        IVoiceChatService chatService,
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

    public IEnumerable<VoiceChatModel> GetPublicVoiceChatsForUserAndRoom(UserModel user, RoomModel room)
    {
        return user.VoiceChats.Where(vc => vc.Room == room && !vc.IsPrivate);
    }

    public IEnumerable<VoiceChatModel> GetPrivateVoiceChatsForUserAndRoom(UserModel user, RoomModel room)
    {
        return user.VoiceChats.Where(vc => vc.Room == room && vc.IsPrivate);
    }

    public async Task<ExceptionalResult> CreatePublicVoiceChatInRoomByUser(RoomModel room, UserModel user, ChatCreateModel createModel, bool asTransaction = true)
    {
        createModel.IsPrivate = false;
        return asTransaction
            ? await this.transactionsWorker.RunAsTransaction(() =>
                this.CreateVoiceChatInRoomForUser(room, user, createModel))
            : await this.CreateVoiceChatInRoomForUser(room, user, createModel);
    }

    public async Task<ExceptionalResult> CreatePrivateVoiceChatInRoomForUsers(RoomModel room, UserModel userCreator, UserModel user, ChatCreateModel createModel, bool asTransaction = true)
    {
        return asTransaction
            ? await this.transactionsWorker.RunAsTransaction(() =>
                this.InnerCreatePrivateVoiceChatInRoomForUsers(room, userCreator, user, createModel))
            : await this.InnerCreatePrivateVoiceChatInRoomForUsers(room, userCreator, user, createModel);
    }

    public async Task<ExceptionalResult> DeletePublicChatInRoomByUser(RoomModel room, UserModel user, VoiceChatModel voiceChat, bool asTransaction = true)
    {
        return asTransaction
            ? await this.transactionsWorker.RunAsTransaction(() =>
                this.DeleteChatInRoomForUser(room, user, voiceChat))
            : await this.DeleteChatInRoomForUser(room, user, voiceChat);
    }

    public async Task<ExceptionalResult> DeletePrivateChatInRoomByUser(RoomModel room, UserModel user, VoiceChatModel voiceChat, bool asTransaction = true)
    {
        return asTransaction
            ? await this.transactionsWorker.RunAsTransaction(() =>
                this.DeleteChatInRoomForUser(room, user, voiceChat, Role.MEMBER))
            : await this.DeleteChatInRoomForUser(room, user, voiceChat, Role.MEMBER);
    }

    public async Task<ExceptionalResult> UpdateVoiceChatInRoomByUser(RoomModel room, UserModel user, ChatEditModel editModel, bool asTransaction = true)
    {
        return asTransaction
            ? await this.transactionsWorker.RunAsTransaction(() =>
                this.InnerUpdateVoiceChatInRoomByUser(room, user, editModel))
            : await this.InnerUpdateVoiceChatInRoomByUser(room, user, editModel);
    }

    public async Task<ExceptionalResult> AddUserToPublicVoiceChatInRoomByUser(RoomModel room, UserModel userToAdd, UserModel user, VoiceChatModel voiceChatModel, bool asTransaction = true)
    {
        return asTransaction
            ? await this.transactionsWorker.RunAsTransaction(() =>
                this.InnerAddUserToPublicVoiceChatInRoomByUser(room, userToAdd, user, voiceChatModel))
            : await this.InnerAddUserToPublicVoiceChatInRoomByUser(room, userToAdd, user, voiceChatModel);
    }

    public async Task<ExceptionalResult> RemoveUserFromPublicVoiceChatInRoomByUser(RoomModel room, UserModel userToRemove, UserModel user, VoiceChatModel voiceChatModel, bool asTransaction = true)
    {
        return asTransaction
            ? await this.transactionsWorker.RunAsTransaction(() =>
                this.InnerRemoveUserFromPublicVoiceChatInRoomByUser(room, userToRemove, user, voiceChatModel))
            : await this.InnerRemoveUserFromPublicVoiceChatInRoomByUser(room, userToRemove, user, voiceChatModel);
    }

    private async Task<ExceptionalResult> InnerUpdateVoiceChatInRoomByUser(RoomModel room, UserModel user, ChatEditModel editModel)
    {
        var chat = await this.chatService.GetVoiceChatById(editModel.Id);
        var roleResult = await this.ValidateRoomUserChatRole(room, user, chat);
        if (!roleResult.IsSuccess)
        {
            return roleResult;
        }

        var updateModel = this.MapEditModelToUpdateModel(editModel);

        return await this.chatService.Update(updateModel);
    }

    private async Task<ExceptionalResult> InnerAddUserToPublicVoiceChatInRoomByUser(RoomModel room, UserModel userToAdd, UserModel user, VoiceChatModel voiceChatModel)
    {
        var role1Result = await this.ValidateRoomUserChatRole(room, user, voiceChatModel);
        if (!role1Result.IsSuccess)
        {
            return role1Result;
        }

        var role2Result = await this.CheckRoomUserRole(room, userToAdd, Role.MEMBER);
        if (!role2Result.IsSuccess)
        {
            return role2Result;
        }

        voiceChatModel.Users.Add(userToAdd);
        var updateResult = await this.UpdateChatUsers(voiceChatModel);

        return !updateResult.IsSuccess ? updateResult : new ExceptionalResult();
    }

    private async Task<ExceptionalResult> InnerRemoveUserFromPublicVoiceChatInRoomByUser(RoomModel room, UserModel userToRemove, UserModel user, VoiceChatModel voiceChatModel)
    {
        var role1Result = await this.ValidateRoomUserChatRole(room, user, voiceChatModel);
        if (!role1Result.IsSuccess)
        {
            return role1Result;
        }

        var role2Result = await this.ValidateRoomUserChatRole(room, userToRemove, voiceChatModel, Role.MEMBER);
        if (!role2Result.IsSuccess)
        {
            return role2Result;
        }

        voiceChatModel.Users.Remove(userToRemove);
        var updateResult = await this.UpdateChatUsers(voiceChatModel);

        return !updateResult.IsSuccess ? updateResult : new ExceptionalResult();
    }

    private async Task<ExceptionalResult> InnerCreatePrivateVoiceChatInRoomForUsers(RoomModel room, UserModel userCreator, UserModel user, ChatCreateModel createModel)
    {
        createModel.IsPrivate = true;
        var result = await this.CreateVoiceChatInRoomForUser(room, userCreator, createModel, Role.MEMBER);
        if (!result.IsSuccess)
        {
            return result;
        }

        user.VoiceChats.Add(result.Value);

        return await this.UpdateUserChats(user);
    }

    private async Task<OptionalResult<VoiceChatModel>> CreateVoiceChatInRoomForUser(RoomModel room, UserModel user, ChatCreateModel createModel, Role minRole = Role.ADMIN)
    {
        var roleResult = await this.CheckRoomUserRole(room, user, minRole);
        if (!roleResult.IsSuccess)
        {
            return new OptionalResult<VoiceChatModel>(roleResult);
        }

        var validationResult = this.chatValidationService.ValidateCreateModel(createModel);
        if (!validationResult.IsSuccess)
        {
            return new OptionalResult<VoiceChatModel>(validationResult);
        }

        var chatResult = await this.chatService.Create(createModel);
        if (!chatResult.IsSuccess)
        {
            return chatResult;
        }

        var chat = chatResult.Value;
        user.VoiceChats.Add(chat);
        var userResult = await this.UpdateUserChats(user);
        if (!userResult.IsSuccess)
        {
            return new OptionalResult<VoiceChatModel>(userResult);
        }

        room.VoiceChats.Add(chat);
        var roomResult = await this.UpdateRoomChats(room);

        return !roomResult.IsSuccess ? new OptionalResult<VoiceChatModel>(roomResult) : new OptionalResult<VoiceChatModel>(chat);
    }

    private async Task<ExceptionalResult> DeleteChatInRoomForUser(RoomModel room, UserModel user, VoiceChatModel textChat, Role minRole = Role.ADMIN)
    {
        var roleResult = await this.ValidateRoomUserChatRole(room, user, textChat, minRole);
        if (!roleResult.IsSuccess)
        {
            return roleResult;
        }

        room.VoiceChats.Remove(textChat);
        var roomResult = await this.UpdateRoomChats(room);
        if (!roomResult.IsSuccess)
        {
            return roomResult;
        }

        foreach (var chatUser in textChat.Users)
        {
            chatUser.VoiceChats.Remove(textChat);
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
            VoiceChats = user.VoiceChats,
            IsActive = user.IsActive,
        };

        return await this.userService.Update(updateModel);
    }

    private async Task<OptionalResult<VoiceChatModel>> UpdateChatUsers(VoiceChatModel chat)
    {
        var updateModel = new ChatUpdateModel()
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
            VoiceChats = room.VoiceChats,
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

    private ExceptionalResult CheckUserInChat(UserModel user, VoiceChatModel chat)
    {
        return chat.Users.Contains(user)
            ? new ExceptionalResult()
            : new ExceptionalResult(false, $"User {user.Id} does not belong to chat {chat.Id}");
    }

    private async Task<ExceptionalResult> ValidateRoomUserChatRole(RoomModel room, UserModel user, VoiceChatModel chat, Role minRole = Role.ADMIN)
    {
        var result = await this.CheckRoomUserRole(room, user, minRole);

        return result.IsSuccess ? this.CheckUserInChat(user, chat) : result;
    }

    private ChatUpdateModel MapEditModelToUpdateModel(ChatEditModel editModel)
    {
        var mapperConfiguration = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ChatEditModel, ChatUpdateModel>().ForAllMembers(opt => opt.AllowNull());
        });
        var mapper = new Mapper(mapperConfiguration);
        return mapper.Map<ChatUpdateModel>(editModel);
    }
}