using Core.Models.RoomModels;

namespace Core.Models.UserModels
{
    public class UserModel : BaseModel
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string HashedPassword { get; set; }

        public bool IsActive { get; set; }

        public ICollection<RoomModel> Rooms { get; set; } = new HashSet<RoomModel>();

        public ICollection<RoleModel> Roles { get; set; } = new HashSet<RoleModel>();
    }
}
