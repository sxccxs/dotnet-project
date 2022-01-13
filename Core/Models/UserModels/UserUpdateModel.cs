namespace Core.Models.UserModels
{
    public class UserUpdateModel
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public bool IsActive { get; set; }

        public List<int> Rooms { get; set; }

        public List<int> Roles { get; set; }
    }
}
