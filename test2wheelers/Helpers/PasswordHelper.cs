using System.Security.Cryptography;

namespace test2wheelers.Helpers
{
    public class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);

            // Derive key using PBKDF2 (HMAC-SHA1, 10000 iterations, 20 bytes length)
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA1);
            byte[] hash = pbkdf2.GetBytes(20);

            // Combine salt + hash
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            // Convert to Base64 for storage
            return Convert.ToBase64String(hashBytes);
        }

        // Verify entered password
        public static bool VerifyPassword(string enteredPassword, string storedHash)
        {
            // Decode Base64 string
            byte[] hashBytes = Convert.FromBase64String(storedHash);

            // Extract salt
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            // Recompute hash with same parameters
            using var pbkdf2 = new Rfc2898DeriveBytes(enteredPassword, salt, 10000, HashAlgorithmName.SHA1);
            byte[] hash = pbkdf2.GetBytes(20);

            // Compare stored hash and new hash securely
            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                    return false;
            }

            return true;
        }
    }
}
