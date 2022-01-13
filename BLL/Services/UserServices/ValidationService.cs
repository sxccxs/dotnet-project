using System.Net.Mail;
using BLL.Abstractions.Interfaces.UserInterfaces;
using Core.Exceptions;
using Core.Models.UserModels;

namespace BLL.Services.UserServices
{
    internal class ValidationService : IValidationService
    {
        private char[] CapitalAlphabet => "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        private char[] LowerAlphabet => "abcdefghijklmnopqrstuvwxyz".ToCharArray();

        private char[] Numbers => "0123456789".ToCharArray();

        private char[] Symbols => "_!@#$%^&*".ToCharArray();

        private int MaxUserNameLength => 50;

        private int MaxEmailLength => 255;

        private int MaxPasswordLength => 80;

        private int MinPasswordLength => 8;

        public void Validate(UserRegistrationModel user)
        {
            this.ValidateUserName(user.UserName);
            this.ValidateEmail(user.Email);
            this.ValidatePassword(user.Password);
            this.ValidateRePassword(user.Password, user.RePassword);
        }

        private void ValidateUserName(string username)
        {
            if (username is null || string.IsNullOrWhiteSpace(username))
            {
                throw new ValidationException("User name can't be empty");
            }
            else if (username.Length > this.MaxUserNameLength)
            {
                throw new ValidationException($"User name can't be longer then {this.MaxUserNameLength} symbols");
            }
        }

        private void ValidateEmail(string email)
        {
            try
            {
                var mail = new MailAddress(email);
            }
            catch (Exception ex)
            {
                throw new ValidationException(ex.Message);
            }

            if (email.Length > this.MaxEmailLength)
            {
                throw new ValidationException($"Email can't be longer then {this.MaxEmailLength} symbols");
            }
        }

        private void ValidatePassword(string password)
        {
            if (password.Length < this.MinPasswordLength && password.Length > this.MaxPasswordLength)
            {
                throw new ValidationException($"Password can't be less then {this.MinPasswordLength} " +
                                              $"or more then {this.MaxPasswordLength} symbols.");
            }

            if (!password.Any(x => this.CapitalAlphabet.Contains(x)) ||
                !password.Any(x => this.LowerAlphabet.Contains(x)) ||
                !password.Any(x => this.Numbers.Contains(x)) ||
                !password.Any(x => this.Symbols.Contains(x)))
            {
                throw new ValidationException("Password must contain at least one capital, one small letter, one number and one symbol.");
            }
            else if (password.Any(x => !(this.CapitalAlphabet.Contains(x) ||
                                        this.LowerAlphabet.Contains(x) ||
                                        this.Numbers.Contains(x) ||
                                        this.Symbols.Contains(x))))
            {
                throw new ValidationException($"Password can contain only capital and small letter, " +
                                              $"numbers and {string.Join(", ", this.Symbols)}.");
            }
        }

        private void ValidateRePassword(string password, string rePassword)
        {
            if (password != rePassword)
            {
                throw new ValidationException("Passwords must match.");
            }
        }
    }
}
