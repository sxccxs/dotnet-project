using BLL.Abstractions.Interfaces.UserInterfaces;
using Core.DataClasses;
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

        public ExceptionalResult CheckToken(UserModel user, string token)
        {
            if (user is null || string.IsNullOrWhiteSpace(token))
            {
                return new ExceptionalResult(false, "User or token is null");
            }

            long timestamp;

            try
            {
                var timestampBase64 = token.Split("-")[0];
                timestamp = long.Parse(Base64UrlEncoder.Decode(timestampBase64));
            }
            catch (Exception ex)
            {
                return new ExceptionalResult(false, ex.Message);
            }

            if (this.MakeTokenWithTimeStamp(user, timestamp) != token)
            {
                return new ExceptionalResult(false, "Invalid token given");
            }

            if (this.GetNumberOfSeconds(DateTime.UtcNow) - timestamp > this.appSettings.TokensExpirationTime * 24 * 3600)
            {
                return new ExceptionalResult(false, "Token is outdated");
            }

            return new ExceptionalResult();
        }

        public OptionalResult<int> GetIdFromUidb64(string uidb64)
        {
            if (int.TryParse(Base64UrlEncoder.Decode(uidb64), out int result))
            {
                return new OptionalResult<int>(result);
            }

            return new OptionalResult<int>(false, "Invalid uidb64");
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
