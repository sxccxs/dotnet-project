namespace Core.Models.RoomModels
{
    public class RoomModel : BaseModel
    {
        public string Name { get; set; }

        public List<int> Users { get; set; }

        public List<int> Roles { get; set; }

        public List<int> Chats { get; set; }
    }
}
