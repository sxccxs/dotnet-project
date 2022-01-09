using System.Security.Cryptography;
using System.Text;
using BLL.Abstractions.Interfaces;

namespace BLL.Services
{
    internal class SHA256HashingService : IHashingService
    {
        public string Hash(string key)
        {
            using (var sha256Hasher = SHA256.Create())
            {
                var hashed = sha256Hasher.ComputeHash(Encoding.UTF8.GetBytes(key));

                return BitConverter.ToString(hashed).Replace("-", string.Empty).ToLower();
            }
        }
    }
}
