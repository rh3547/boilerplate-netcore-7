using System.Security.Cryptography;
using Nukleus.Application.Common.Services;

namespace Nukleus.Infrastructure.Common.Authentication
{
    // https://stackoverflow.com/questions/4181198/how-to-hash-a-password
    public class HashingService : IHashingService
    {
        private const int _saltSize = 16; // 128 bits
        private const int _keySize = 32; // 256 bits
        private const int _iterations = 100000;
        private static readonly HashAlgorithmName _algorithm = HashAlgorithmName.SHA256;

        private const char segmentDelimiter = ':';
        public string Hash(string input)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(_saltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                input,
                salt,
                _iterations,
                _algorithm,
                _keySize
            );
            return string.Join(
                segmentDelimiter,
                Convert.ToHexString(hash),
                Convert.ToHexString(salt),
                _iterations,
                _algorithm
            );
        }

        public bool Verify(string input, string hashString)
        {
            string[] segments = hashString.Split(segmentDelimiter);
            byte[] hash = Convert.FromHexString(segments[0]);
            byte[] salt = Convert.FromHexString(segments[1]);
            int iterations = int.Parse(segments[2]);
            HashAlgorithmName algorithm = new HashAlgorithmName(segments[3]);
            byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(
                input,
                salt,
                iterations,
                algorithm,
                hash.Length
            );
            return CryptographicOperations.FixedTimeEquals(inputHash, hash);
        }
    }
}