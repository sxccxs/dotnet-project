using Core.Enums;

namespace Core.Models
{
    public class RoleModel : BaseModel
    {
        public Role Role { get; set; }

        public int UserId { get; set; }

        public int RoomId { get; set; }
    }
}
