using BLL.Abstractions.Interfaces.UserInterfaces;
using Core.Exceptions;
using Core.Models.UserModels;
using Core.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BLL.Services.UserServices
{
    internal class TokenGeneratorService : ITokenGeneratorService
    {
        private readonly AppSettings appSettings;

        private readonly IHashingService hashingService;

        public TokenGeneratorService(IHashingService hashingService, IOptions<AppSettings> appSettings)
        {
            this.appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));
            this.hashingService = hashingService;
        }

        public void CheckToken(UserModel user, string token)
        {
            if (user is null || token is null)
            {
                throw new ArgumentNullException("User or token is null");
            }

            try
            {
                var timestampBase64 = token.Split("-")[0];
                var timestamp = long.Parse(Base64UrlEncoder.Decode(timestampBase64));

                if (this.MakeTokenWithTimeStamp(user, timestamp) != token)
                {
                    throw new InvalidTokenException("Invalid token given");
                }

                if (this.GetNumberOfSeconds(DateTime.UtcNow) - timestamp > this.appSettings.TokensExpirationTime * 24 * 3600)
                {
                    throw new InvalidTokenException("Token is outdated");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidTokenException(ex.Message);
            }
        }

        public int GetIdFromUidb64(string uidb64)
        {
            return int.Parse(Base64UrlEncoder.Decode(uidb64));
        }

        public string GetToken(UserModel user)
        {
            return this.MakeTokenWithTimeStamp(user, this.GetNumberOfSeconds(DateTime.UtcNow));
        }

        public string GetUidb64(UserModel user)
        {
            return Base64UrlEncoder.Encode(user.Id.ToString());
        }

        private string MakeTokenWithTimeStamp(UserModel user, long timestamp)
        {
            var timestampBase64 = Base64UrlEncoder.Encode(timestamp.ToString());
            var hashString = this.hashingService.Hash(this.MakeHashString(user, timestamp))
                .Where((_, index) => index % 2 == 0); // select second items to shorten url
            return $"{timestampBase64}-{string.Join(string.Empty, hashString)}";
        }

        private string MakeHashString(UserModel user, long timestamp)
        {
            return $"{user.Id}{user.Email}{user.HashedPassword}{timestamp}{this.appSettings.SecretKey}";
        }

        private long GetNumberOfSeconds(DateTime dateTime)
        {
            return (long)(dateTime - new DateTime(1970, 1, 1)).TotalSeconds;
        }
    }
}
