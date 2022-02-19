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

    Task<ExceptionalResult> CreatePublicVoiceChatInRoomByUser(RoomModel room, UserModel user, ChatCreateModel createModel);

    Task<ExceptionalResult> CreatePrivateVoiceChatInRoomForUsers(RoomModel room, UserModel userCreator, UserModel user, ChatCreateModel createModel);

    Task<ExceptionalResult> DeletePublicChatInRoomByUser(RoomModel room, UserModel user, VoiceChatModel voiceChat);

    Task<ExceptionalResult> DeletePrivateChatInRoomByUser(RoomModel room, UserModel user, VoiceChatModel voiceChat);

    Task<ExceptionalResult> UpdateVoiceChatInRoomByUser(RoomModel room, UserModel user, ChatEditModel editModel);

    Task<ExceptionalResult> AddUserToPublicVoiceChatInRoomByUser(RoomModel room, UserModel userToAdd, UserModel user, VoiceChatModel voiceChatModel);

    Task<ExceptionalResult> RemoveUserFromPublicVoiceChatInRoomByUser(RoomModel room, UserModel userToRemove, UserModel user, VoiceChatModel voiceChatModel);
}
