// Bank_Configuration_Portal.Common/Security/PasswordHasher.cs
using System;
using System.Security.Cryptography;

namespace Bank_Configuration_Portal.Common.Security
{
    public static class PasswordHasher
    {
        private const int DefaultIterations = 150_000;
        private const int SaltSize = 16;   
        private const int HashSize = 32;  

        public static (byte[] hash, byte[] salt, int iterations) Hash(string password, int? iterations = null)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));

            var iters = iterations ?? DefaultIterations;
            var salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            var hash = Pbkdf2(password, salt, iters, HashSize);
            return (hash, salt, iters);
        }

        public static bool Verify(string password, byte[] storedHash, byte[] storedSalt, int iterations)
        {
            if (password == null || storedHash == null || storedSalt == null) return false;

            var computed = Pbkdf2(password, storedSalt, iterations, storedHash.Length);
            return ConstantTimeEquals(computed, storedHash);
        }

        private static byte[] Pbkdf2(string password, byte[] salt, int iterations, int outputBytes)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                return pbkdf2.GetBytes(outputBytes);
            }
        }

        private static bool ConstantTimeEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;
            int diff = 0;
            for (int i = 0; i < a.Length; i++)
                diff |= a[i] ^ b[i];
            return diff == 0;
        }
    }
}
