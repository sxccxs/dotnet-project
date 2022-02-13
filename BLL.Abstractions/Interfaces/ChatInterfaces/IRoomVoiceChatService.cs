using Core.DataClasses;
using Core.Enums;
using Core.Models.ChatModels;
using Core.Models.RoomModels;
using Core.Models.UserModels;

namespace BLL.Abstractions.Interfaces.ChatInterfaces;

public interface IRoomVoiceChatService
{
    IEnumerable<VoiceChatModel> GetPublicVoiceChatsForUserAndRoom(UserModel user, RoomModel room);

    IEnumerable<VoiceChatModel> GetPrivateVoiceChatsForUserAndRoom(UserModel user, RoomModel room);

    Task<ExceptionalResult> CreatePublicVoiceChatInRoomByUser(RoomModel room, UserModel user, ChatCreateModel createModel, bool asTransaction = true);

    Task<ExceptionalResult> CreatePrivateVoiceChatInRoomForUsers(RoomModel room, UserModel userCreator, UserModel user, ChatCreateModel createModel, bool asTransaction = true);

    Task<ExceptionalResult> DeletePublicChatInRoomByUser(RoomModel room, UserModel user, VoiceChatModel voiceChat, bool asTransaction = true);

    Task<ExceptionalResult> DeletePrivateChatInRoomByUser(RoomModel room, UserModel user, VoiceChatModel voiceChat, bool asTransaction = true);

    Task<ExceptionalResult> UpdateVoiceChatInRoomByUser(RoomModel room, UserModel user, ChatEditModel editModel, bool asTransaction = true);

    Task<ExceptionalResult> AddUserToPublicVoiceChatInRoomByUser(RoomModel room, UserModel userToAdd, UserModel user, VoiceChatModel voiceChatModel, bool asTransaction = true);

    Task<ExceptionalResult> RemoveUserFromPublicVoiceChatInRoomByUser(RoomModel room, UserModel userToRemove, UserModel user, VoiceChatModel voiceChatModel, bool asTransaction = true);
}