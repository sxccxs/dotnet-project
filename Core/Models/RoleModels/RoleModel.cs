using System.ComponentModel.DataAnnotations;
using Core.Models.RoomModels;
using Core.Models.UserModels;

namespace Core.Models.RoleModels
{
    public class RoleModel : BaseModel
    {
        [Required]
        public RoleTypeModel RoleType { get; set; }

        [Required]
        public UserModel User { get; set; }

        [Required]
        public RoomModel Room { get; set; }
    }
}
