using Core.Models.UserModels;

namespace Core.Models.RoomModels
{
    public class RoomModel : BaseModel
    {
        public string Name { get; set; }

        public ICollection<RoleModel> Roles { get; set; } = new HashSet<RoleModel>();

        public ICollection<UserModel> Users { get; set; } = new HashSet<UserModel>();
    }
}
