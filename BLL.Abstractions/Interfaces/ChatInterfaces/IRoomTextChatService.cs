using Core.DataClasses;
using Core.Enums;
using Core.Models.ChatModels;
using Core.Models.RoomModels;
using Core.Models.UserModels;

namespace BLL.Abstractions.Interfaces.ChatInterfaces;

public interface IRoomTextChatService
{
    IEnumerable<TextChatModel> GetPublicTextChatsForUserAndRoom(UserModel user, RoomModel room);

    IEnumerable<TextChatModel> GetPrivateTextChatsForUserAndRoom(UserModel user, RoomModel room);

    ExceptionalResult CheckUserInChat(UserModel user, TextChatModel chat);

    Task<ExceptionalResult> ValidateRoomUserChatRole(RoomModel room, UserModel user, TextChatModel chat, RoleType minRole = RoleType.Admin);

    Task<ExceptionalResult> CreatePublicTextChatInRoomByUser(RoomModel room, UserModel user, ChatCreateModel createModel);

    Task<ExceptionalResult> CreatePrivateTextChatInRoomForUsers(RoomModel room, UserModel userCreator, UserModel user, ChatCreateModel createModel);

    Task<ExceptionalResult> UpdateTextChatInRoomByUser(RoomModel room, UserModel user, ChatEditModel editModel);

    Task<ExceptionalResult> DeletePublicChatInRoomByUser(RoomModel room, UserModel user, TextChatModel textChat);

    Task<ExceptionalResult> DeletePrivateChatInRoomByUser(RoomModel room, UserModel user, TextChatModel textChat);

    Task<ExceptionalResult> AddUserToPublicTextChatInRoomByUser(RoomModel room, UserModel userToAdd, UserModel user, TextChatModel textChatModel);

    Task<ExceptionalResult> RemoveUserFromPublicTextChatInRoomByUser(RoomModel room, UserModel userToRemove, UserModel user, TextChatModel textChatModel);

    Task<ExceptionalResult> ClearPublicChatHistory(UserModel user, TextChatModel chat);
}
