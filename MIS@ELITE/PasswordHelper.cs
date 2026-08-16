using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MIS_ELITE
{
    internal class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            //Generate a random salt
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            } // Derive the key (hash) using PBKDF2
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32);
            // Combine salt + hash for storage
            byte[] hashBytes = new byte[48];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 32);
            // Convert to Base64 string
            return Convert.ToBase64String(hashBytes);
        }
        public static bool VerifyPassword(string password, string storedHash)
        {
            byte[] hashBytes = Convert.FromBase64String(storedHash);
            // Extract salt
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            // Extract stored hash
            byte[] storedSubHash = new byte[32];
            Array.Copy(hashBytes, 16, storedSubHash, 0, 32);
            // Hash the input password with the same salt
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
            byte[] newHash = pbkdf2.GetBytes(32);
            // Compare byte arrays
            for (int i = 0; i < 32; i++)
            {
                if (newHash[i] != storedSubHash[i])
                    return false;
            }
            return true;
        }
    }
}
