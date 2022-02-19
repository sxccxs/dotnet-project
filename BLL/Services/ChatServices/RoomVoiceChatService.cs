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

public class RoomVoiceChatService : IRoomVoiceChatService
{
    private readonly IUserRoomRoleService roleService;

    private readonly IVoiceChatService chatService;

    private readonly IChatValidationService chatValidationService;

    private readonly IAuditService auditService;

    private readonly ITransactionsWorker transactionsWorker;

    public RoomVoiceChatService(
        ITransactionsWorker transactionsWorker,
        IAuditService auditService,
        IUserRoomRoleService roleService,
        IVoiceChatService chatService,
        IChatValidationService chatValidationService)
    {
        this.roleService = roleService;
        this.auditService = auditService;
        this.chatService = chatService;
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

    public async Task<ExceptionalResult> CreatePublicVoiceChatInRoomByUser(RoomModel room, UserModel user, ChatCreateModel createModel)
    {
        return await this.transactionsWorker.RunAsTransaction(() =>
            this.InnerCreatePublicVoiceChatInRoomByUser(room, user, createModel));
    }

    public async Task<ExceptionalResult> CreatePrivateVoiceChatInRoomForUsers(RoomModel room, UserModel userCreator, UserModel user, ChatCreateModel createModel)
    {
        return await this.transactionsWorker.RunAsTransaction(() =>
            this.InnerCreatePrivateVoiceChatInRoomForUsers(room, userCreator, user, createModel));
    }

    public async Task<ExceptionalResult> DeletePublicChatInRoomByUser(RoomModel room, UserModel user, VoiceChatModel voiceChat)
    {
        return await this.transactionsWorker.RunAsTransaction(() => this.DeleteChatInRoomForUser(room, user, voiceChat));
    }

    public async Task<ExceptionalResult> DeletePrivateChatInRoomByUser(RoomModel room, UserModel user, VoiceChatModel voiceChat)
    {
        return await this.transactionsWorker.RunAsTransaction(() =>
            this.DeleteChatInRoomForUser(room, user, voiceChat, RoleType.Member));
    }

    public async Task<ExceptionalResult> UpdateVoiceChatInRoomByUser(RoomModel room, UserModel user, ChatEditModel editModel)
    {
        return await this.transactionsWorker.RunAsTransaction(() =>
            this.InnerUpdateVoiceChatInRoomByUser(room, user, editModel));
    }

    public async Task<ExceptionalResult> AddUserToPublicVoiceChatInRoomByUser(RoomModel room, UserModel userToAdd, UserModel user, VoiceChatModel voiceChatModel)
    {
        return await this.transactionsWorker.RunAsTransaction(() =>
            this.InnerAddUserToPublicVoiceChatInRoomByUser(room, userToAdd, user, voiceChatModel));
    }

    public async Task<ExceptionalResult> RemoveUserFromPublicVoiceChatInRoomByUser(RoomModel room, UserModel userToRemove, UserModel user, VoiceChatModel voiceChatModel)
    {
        return await this.transactionsWorker.RunAsTransaction(() =>
            this.InnerRemoveUserFromPublicVoiceChatInRoomByUser(room, userToRemove, user, voiceChatModel));
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

        var result = await this.chatService.Update(updateModel);
        if (!result.IsSuccess)
        {
            return result;
        }

        var record = new CreateAuditRecordModel()
        {
            ActionType = ActionType.EditVoiceChatInfo,
            Actor = user,
            Room = room,
            VoiceChat = chat,
        };

        return await this.auditService.CreateAuditRecord(record);
    }

    private async Task<ExceptionalResult> InnerAddUserToPublicVoiceChatInRoomByUser(RoomModel room, UserModel userToAdd, UserModel user, VoiceChatModel voiceChatModel)
    {
        var role1Result = await this.ValidateRoomUserChatRole(room, user, voiceChatModel);
        if (!role1Result.IsSuccess)
        {
            return role1Result;
        }

        var role2Result = await this.CheckRoomUserRole(room, userToAdd, RoleType.Member);
        if (!role2Result.IsSuccess)
        {
            return role2Result;
        }

        voiceChatModel.Users.Add(userToAdd);

        var result = await this.UpdateChatUsers(voiceChatModel);
        if (!result.IsSuccess)
        {
            return result;
        }

        var record = new CreateAuditRecordModel()
        {
            ActionType = ActionType.AddUserToVoiceChat,
            Actor = user,
            Room = room,
            UserUnderAction = userToAdd,
            VoiceChat = voiceChatModel,
        };

        return await this.auditService.CreateAuditRecord(record);
    }

    private async Task<ExceptionalResult> InnerRemoveUserFromPublicVoiceChatInRoomByUser(RoomModel room, UserModel userToRemove, UserModel user, VoiceChatModel voiceChatModel)
    {
        var role1Result = await this.ValidateRoomUserChatRole(room, user, voiceChatModel);
        if (!role1Result.IsSuccess)
        {
            return role1Result;
        }

        var role2Result = await this.ValidateRoomUserChatRole(room, userToRemove, voiceChatModel, RoleType.Member);
        if (!role2Result.IsSuccess)
        {
            return role2Result;
        }

        voiceChatModel.Users.Remove(userToRemove);

        var result = await this.UpdateChatUsers(voiceChatModel);
        if (!result.IsSuccess)
        {
            return result;
        }

        var record = new CreateAuditRecordModel()
        {
            ActionType = ActionType.DeleteUserFromVoiceChat,
            Actor = user,
            Room = room,
            UserUnderAction = userToRemove,
            VoiceChat = voiceChatModel,
        };

        return await this.auditService.CreateAuditRecord(record);
    }

    private async Task<ExceptionalResult> InnerCreatePublicVoiceChatInRoomByUser(RoomModel room, UserModel user, ChatCreateModel createModel)
    {
        createModel.IsPrivate = false;
        createModel.Users = new List<UserModel>() { user };

        var result = await this.CreateVoiceChatInRoomForUser(room, user, createModel);
        if (!result.IsSuccess)
        {
            return result;
        }

        var record = new CreateAuditRecordModel()
        {
            ActionType = ActionType.CreateVoiceChat,
            Actor = user,
            Room = room,
            VoiceChat = result.Value,
        };

        return await this.auditService.CreateAuditRecord(record);
    }

    private async Task<ExceptionalResult> InnerCreatePrivateVoiceChatInRoomForUsers(RoomModel room, UserModel userCreator, UserModel user, ChatCreateModel createModel)
    {
        createModel.IsPrivate = true;
        createModel.Users = new List<UserModel>() { userCreator, user };

        return await this.CreateVoiceChatInRoomForUser(room, userCreator, createModel, RoleType.Member);
    }

    private async Task<OptionalResult<VoiceChatModel>> CreateVoiceChatInRoomForUser(RoomModel room, UserModel user, ChatCreateModel createModel, RoleType minRole = RoleType.Admin)
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

        createModel.Room = room;

        return await this.chatService.Create(createModel);
    }

    private async Task<ExceptionalResult> DeleteChatInRoomForUser(RoomModel room, UserModel user, VoiceChatModel voiceChat, RoleType minRole = RoleType.Admin)
    {
        var roleResult = await this.ValidateRoomUserChatRole(room, user, voiceChat, minRole);
        if (!roleResult.IsSuccess)
        {
            return roleResult;
        }

        var result = await this.chatService.Delete(voiceChat.Id);
        if (!result.IsSuccess || voiceChat.IsPrivate)
        {
            return result;
        }

        var record = new CreateAuditRecordModel()
        {
            ActionType = ActionType.DeleteVoiceChat,
            Actor = user,
            Room = room,
            VoiceChat = voiceChat,
        };

        return await this.auditService.CreateAuditRecord(record);
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

    private async Task<ExceptionalResult> CheckRoomUserRole(RoomModel room, UserModel user, RoleType minRole = RoleType.Admin)
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

    private async Task<ExceptionalResult> ValidateRoomUserChatRole(RoomModel room, UserModel user, VoiceChatModel chat, RoleType minRole = RoleType.Admin)
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
