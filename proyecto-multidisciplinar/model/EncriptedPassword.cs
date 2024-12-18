using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace proyecto_multidisciplinar.model
{
    internal class EncriptedPassword
    {
        public static string HashPassword(string password)
        {
            //generate a unique salt
            byte[] salt = GenerateSalt();

            // generate hash
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(32);

                //combine the hash with the salt
                byte[] hashBytes = new byte[48];
                Array.Copy(salt, 0, hashBytes, 0, 16); // 16 bytes
                Array.Copy(hash, 0, hashBytes, 16, 32); // 32 bytes

                // return as base64
                return Convert.ToBase64String(hashBytes);
            }
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            //convert the hash from base64 to bytes
            
            byte[] hashBytes = Convert.FromBase64String(storedHash);

            // extrat the hash
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            //generate a hash from the password using the original salt
            
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(32);

                for (int i = 0; i < 32; i++)
                {
                    if (hashBytes[i + 16] != hash[i])
                    {
                        return false; // if there is a diference, the password is incorrect
                    }
                }
            }
            return true; // if the hashes are equals, is correct
        }

        private static byte[] GenerateSalt()
        {
            byte[] salt = new byte[16]; // 16 bytes
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt); // fit with randmo bytes
            }
            return salt;
        }
    }
}
