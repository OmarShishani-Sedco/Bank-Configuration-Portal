// Bank_Configuration_Portal.Common/Security/PasswordGenerator.cs
using System.Security.Cryptography;
using System.Text;

namespace Bank_Configuration_Portal.Common.Security
{
    public static class PasswordGenerator
    {
        public static string Generate(int length = 14)
        {
            const string alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789!@#$%^&*()-_=+";
            var bytes = new byte[length];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }

            var sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
                sb.Append(alphabet[bytes[i] % alphabet.Length]);

            return sb.ToString();
        }
    }
}
