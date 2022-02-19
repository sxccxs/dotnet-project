using Core.Enums;
using Core.Models.ChatModels;
using Core.Models.RoleModels;
using Core.Models.RoomModels;
using Core.Models.UserModels;

namespace Core.Models.AuditModels;

public class CreateAuditRecordModel
{
    public RoomModel Room { get; set; }

    public UserModel Actor { get; set; }

    public UserModel UserUnderAction { get; set; }

    public TextChatModel TextChat { get; set; }

    public VoiceChatModel VoiceChat { get; set; }

    public RoleTypeModel OldRole { get; set; }

    public RoleTypeModel NewRole { get; set; }

    public ActionType ActionType { get; set; }
}