using System.Security.Cryptography;

namespace _2whealers.Helpers
{
    public class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            // Generate a salt
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            // Create PBKDF2 hash
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000); // 10,000 iterations
            byte[] hash = pbkdf2.GetBytes(20);

            // Combine salt + hash
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            // Convert to Base64 string
            return Convert.ToBase64String(hashBytes);
        }

        // Verify entered password
        public static bool VerifyPassword(string enteredPassword, string storedHash)
        {
            // Extract bytes
            byte[] hashBytes = Convert.FromBase64String(storedHash);

            // Get salt
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            // Compute hash with stored salt
            var pbkdf2 = new Rfc2898DeriveBytes(enteredPassword, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            // Compare
            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash[i]) return false;
            }

            return true;
        }
    }
}
