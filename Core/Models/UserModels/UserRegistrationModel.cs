using System.ComponentModel.DataAnnotations;

namespace Core.Models.UserModels
{
    public class UserRegistrationModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string RePassword { get; set; }
    }
}
