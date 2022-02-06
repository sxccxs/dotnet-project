using System.ComponentModel.DataAnnotations;
using Core.Models.UserModels;

namespace Core.Models.RoomModels
{
    public class RoomModel : BaseModel
    {
        [Required]
        [MaxLength(32)]
        public string Name { get; set; }

        public ICollection<RoleModel> Roles { get; set; } = new HashSet<RoleModel>();

        public ICollection<UserModel> Users { get; set; } = new HashSet<UserModel>();
    }
}
