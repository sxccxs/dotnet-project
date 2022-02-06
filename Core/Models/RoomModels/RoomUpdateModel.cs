namespace Core.Models.RoomModels
{
    public class RoomUpdateModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<int> Users { get; set; }

        public List<int> Roles { get; set; }

        public List<int> Chats { get; set; }
    }
}
