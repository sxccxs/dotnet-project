namespace Core.Models
{
    public class UserRegistrationModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string RePassword { get; set; }
        public UserRegistrationModel(string userName, string email, string password, string rePassword)
        {
            UserName = userName;
            Email = email;
            Password = password;
            RePassword = rePassword;
        }

    }
}
