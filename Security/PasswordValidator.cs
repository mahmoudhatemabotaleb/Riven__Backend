using System.Text.RegularExpressions;

namespace RivenBackend.Security
{
    public static partial class PasswordValidator
    {
        public static bool IsValid(string password, out string error)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                error = "Password is required.";
                return false;
            }

            if (password.Length < 8)
            {
                error = "Password must be at least 8 characters.";
                return false;
            }

            if (!UppercaseRegex().IsMatch(password))
            {
                error = "Password must contain at least one uppercase letter.";
                return false;
            }

            if (!LowercaseRegex().IsMatch(password))
            {
                error = "Password must contain at least one lowercase letter.";
                return false;
            }

            if (!DigitRegex().IsMatch(password))
            {
                error = "Password must contain at least one digit.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        [GeneratedRegex("[A-Z]")]
        private static partial Regex UppercaseRegex();

        [GeneratedRegex("[a-z]")]
        private static partial Regex LowercaseRegex();

        [GeneratedRegex("[0-9]")]
        private static partial Regex DigitRegex();
    }
}
