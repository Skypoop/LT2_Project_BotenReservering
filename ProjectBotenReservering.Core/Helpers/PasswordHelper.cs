using System;
using System.Security.Cryptography;
using System.Text;

namespace ProjectBotenReservering.Core.Helpers
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                passwordBytes,
                salt,
                100000,
                HashAlgorithmName.SHA256,
                32
            );

            string saltBase64 = Convert.ToBase64String(salt);
            string hashBase64 = Convert.ToBase64String(hash);

            return saltBase64 + "." + hashBase64;
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            string[] parts = storedHash.Split('.');
            if (parts.Length != 2)
                return false;

            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] hash = Convert.FromBase64String(parts[1]);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(
                passwordBytes,
                salt,
                100000,
                HashAlgorithmName.SHA256,
                32
            );

            return CryptographicOperations.FixedTimeEquals(inputHash, hash);
        }
    }
}
