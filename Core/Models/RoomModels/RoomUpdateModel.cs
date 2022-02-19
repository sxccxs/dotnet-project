using Core.Models.AuditModels;
using Core.Models.ChatModels;
using Core.Models.RoleModels;
using Core.Models.UserModels;

namespace Core.Models.RoomModels
{
    public class RoomUpdateModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<UserModel> Users { get; set; }

        public ICollection<RoleModel> Roles { get; set; }

        public ICollection<TextChatModel> TextChats { get; set; }

        public ICollection<VoiceChatModel> VoiceChats { get; set; }

        public ICollection<AuditRecordModel> AuditJournal { get; set; }
    }
}
