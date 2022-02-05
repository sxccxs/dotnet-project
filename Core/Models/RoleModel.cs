using Core.Models.RoomModels;
using Core.Models.UserModels;

namespace Core.Models
{
    public class RoleModel : BaseModel
    {
        public RoleTypeModel RoleType { get; set; }

        public UserModel User { get; set; }

        public RoomModel Room { get; set; }
    }
}
