using System.Security.Cryptography;

namespace RivenBackend.Security
{
    public static class OtpHelper
    {
        public static string GenerateCode() =>
            RandomNumberGenerator.GetInt32(100000, 999999).ToString();

        public static string HashCode(string code) =>
            BCrypt.Net.BCrypt.HashPassword(code);

        public static bool VerifyCode(string code, string storedHash)
        {
            if (storedHash.Length == 6 && storedHash.All(char.IsDigit))
                return storedHash == code;

            return BCrypt.Net.BCrypt.Verify(code, storedHash);
        }
    }
}
