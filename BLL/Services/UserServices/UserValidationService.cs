using System.Net.Mail;
using BLL.Abstractions.Interfaces.UserInterfaces;
using Core.DataClasses;
using Core.Models.UserModels;

namespace BLL.Services.UserServices
{
    internal class UserValidationService : IUserValidationService
    {
        private const int MaxUserNameLength = 50;

        private const int MaxEmailLength = 255;

        private const int MaxPasswordLength = 80;

        private const int MinPasswordLength = 8;

        private char[] PasswordSpecialCharacters => "_!@#$%^&*".ToCharArray();

        public ExceptionalResult Validate(UserRegistrationModel user)
        {
            var results = new ExceptionalResult[]
            {
                this.ValidateUserName(user.UserName),
                this.ValidateEmail(user.Email),
                this.ValidatePassword(user.Password),
                this.ValidateRePassword(user.Password, user.RePassword),
            };

            var incorrectResults = results.Where(r => !r.IsSuccess).ToList();

            return incorrectResults.Any() ? incorrectResults.First() : new ExceptionalResult();
        }

        public ExceptionalResult ValidateUpdateModel(UserEditModel user)
        {
            var results = new List<ExceptionalResult>();
            if (user.UserName is not null)
            {
                results.Add(this.ValidateUserName(user.UserName));
            }

            if (user.Email is not null)
            {
                results.Add(this.ValidateEmail(user.Email));
            }

            if (user.Password is not null)
            {
                results.Add(this.ValidatePassword(user.Password));
            }

            foreach (var result in results)
            {
                if (!result.IsSuccess)
                {
                    return result;
                }
            }

            return new ExceptionalResult();
        }

        private ExceptionalResult ValidateUserName(string username)
        {
            if (username is null || string.IsNullOrWhiteSpace(username))
            {
                return new ExceptionalResult(false, "User name can't be empty");
            }
            else if (username.Length > MaxUserNameLength)
            {
                return new ExceptionalResult(false, $"User name can't be longer then {MaxUserNameLength} symbols");
            }

            return new ExceptionalResult();
        }

        private ExceptionalResult ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return new ExceptionalResult(false, "Email can't be empty");
            }

            try
            {
                var mail = new MailAddress(email);
            }
            catch (Exception ex)
            {
                return new ExceptionalResult(false, ex.Message);
            }

            if (email.Length > MaxEmailLength)
            {
                return new ExceptionalResult(false, $"Email can't be longer then {MaxEmailLength} symbols");
            }

            return new ExceptionalResult();
        }

        private ExceptionalResult ValidatePassword(string password)
        {
            if (password.Length < MinPasswordLength && password.Length > MaxPasswordLength)
            {
                return new ExceptionalResult(false, $"Password can't be less then {MinPasswordLength} or more then {MaxPasswordLength} symbols.");
            }

            if (!password.Any(this.IsUpperAsciiLetter) ||
                !password.Any(this.IsLowerAsciiLetter) ||
                !password.Any(this.IsAsciiDigit) ||
                !password.Any(this.PasswordSpecialCharacters.Contains))
            {
                return new ExceptionalResult(false, "Password must contain at least one capital, one small letter, one number and one symbol.");
            }
            else if (password.Any(x => !(this.IsUpperAsciiLetter(x) ||
                                        this.IsLowerAsciiLetter(x) ||
                                        this.IsAsciiDigit(x) ||
                                        this.PasswordSpecialCharacters.Contains(x))))
            {
                return new ExceptionalResult(false, $"Password can contain only capital and small letter, numbers and {string.Join(", ", this.PasswordSpecialCharacters)}.");
            }

            return new ExceptionalResult();
        }

        private bool IsUpperAsciiLetter(char character) => char.IsAscii(character) &&
                                                           char.IsLetter(character) &&
                                                           char.IsUpper(character);

        private bool IsLowerAsciiLetter(char character) => char.IsAscii(character) &&
                                                           char.IsLetter(character) &&
                                                           char.IsLower(character);

        private bool IsAsciiDigit(char character) => char.IsAscii(character) &&
                                                      char.IsDigit(character);

        private ExceptionalResult ValidateRePassword(string password, string rePassword)
        {
            if (password != rePassword)
            {
                return new ExceptionalResult(false, "Passwords must match.");
            }

            return new ExceptionalResult();
        }
    }
}
