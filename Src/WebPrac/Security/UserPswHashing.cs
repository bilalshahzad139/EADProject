using PMS.Entities;
using System;
using System.Security.Cryptography;
using System.Text;

namespace WebPrac.Security
{
    public static class UserPswHashing
    {
        private static int SaltSize = 16;

        public static string CreateSalt()
        {
            //Generate a cryptographic random number.
            var saltCryptoGenerator = new RNGCryptoServiceProvider();
            var salt = new byte[UserPswHashing.SaltSize];
            saltCryptoGenerator.GetBytes(salt);
            return Convert.ToBase64String(salt);
        }

        public static void GenerateHash(UserDTO user)
        {
            var bytes = Encoding.UTF8.GetBytes(user.Password + user.PswSalt);
            var sHA256ManagedString = new SHA256Managed();
            var hash = sHA256ManagedString.ComputeHash(bytes);
            user.Password = Convert.ToBase64String(hash);
        }

    }
}