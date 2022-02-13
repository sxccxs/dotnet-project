using Core.Models.ChatModels;
using Core.Models.RoleModels;
using Core.Models.RoomModels;

namespace Core.Models.UserModels
{
    public class UserUpdateModel
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public bool IsActive { get; set; }

        public ICollection<RoomModel> Rooms { get; set; }

        public ICollection<RoleModel> Roles { get; set; }

        public ICollection<TextChatModel> TextChats { get; set; }
    }
}
