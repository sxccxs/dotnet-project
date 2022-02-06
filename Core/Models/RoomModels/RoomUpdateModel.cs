using Core.Models.UserModels;

namespace Core.Models.RoomModels
{
    public class RoomUpdateModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<UserModel> Users { get; set; }

        public ICollection<RoleModel> Roles { get; set; }

        // public List<int> Chats { get; set; }
    }
}
