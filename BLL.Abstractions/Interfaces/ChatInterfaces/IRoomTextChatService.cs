using Core.DataClasses;
using Core.Models.ChatModels;
using Core.Models.RoomModels;
using Core.Models.UserModels;

namespace BLL.Abstractions.Interfaces.ChatInterfaces;

public interface IRoomTextChatService
{
    IEnumerable<TextChatModel> GetPublicTextChatsForUserAndRoom(UserModel user, RoomModel room);

    IEnumerable<TextChatModel> GetPrivateTextChatsForUserAndRoom(UserModel user, RoomModel room);

    Task<ExceptionalResult> CreatePublicTextChatInRoomByUser(RoomModel room, UserModel user, ChatCreateModel createModel, bool asTransaction = true);

    Task<ExceptionalResult> CreatePrivateTextChatInRoomForUsers(RoomModel room, UserModel userCreator, UserModel user, ChatCreateModel createModel, bool asTransaction = true);

    Task<ExceptionalResult> UpdateTextChatInRoomByUser(RoomModel room, UserModel user, ChatEditModel editModel, bool asTransaction = true);

    Task<ExceptionalResult> DeletePublicChatInRoomByUser(RoomModel room, UserModel user, TextChatModel textChat, bool asTransaction = true);

    Task<ExceptionalResult> DeletePrivateChatInRoomByUser(RoomModel room, UserModel user, TextChatModel textChat, bool asTransaction = true);

    Task<ExceptionalResult> AddUserToPublicTextChatInRoomByUser(RoomModel room, UserModel userToAdd, UserModel user, TextChatModel textChatModel, bool asTransaction = true);

    Task<ExceptionalResult> RemoveUserFromPublicTextChatInRoomByUser(RoomModel room, UserModel userToRemove, UserModel user, TextChatModel textChatModel, bool asTransaction = true);
}