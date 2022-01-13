namespace Core.Models.UserModels
{
    public class UserModel : BaseModel
    {
        public UserModel()
        {
            this.Rooms = new List<int>();
            this.Roles = new List<int>();
        }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string HashedPassword { get; set; }

        public bool IsActive { get; set; }

        public List<int> Rooms { get; set; }

        public List<int> Roles { get; set; }
    }
}
