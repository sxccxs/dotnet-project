namespace Core.Models
{
    public class UserRegistrationModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string RePassword { get; set; }
        public UserRegistrationModel(string username, string email, string password, string rePassword)
        {
            Username = username;
            Email = email;
            Password = password;
            RePassword = rePassword;
        }

    }
}
