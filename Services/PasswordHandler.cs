using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace UserService.Services
{
    public class PasswordHandler
    {
        private Regex hasNumber = new Regex(@"[0-9]+");
        private Regex hasUpperChar = new Regex(@"[A-Z]+");
        private Regex hasMinimum8Chars = new Regex(@".{8,}");

        public string Encode (string input)
        {
            using (SHA256 sha256hash = SHA256.Create())
            {
                byte[] bytes = sha256hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder stringBuilder = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {
                    stringBuilder.Append(bytes[i].ToString());
                }

                return stringBuilder.ToString();

            }
        }

        public bool Validate (string input)
        {
            return hasNumber.IsMatch(input) && hasUpperChar.IsMatch(input) && hasMinimum8Chars.IsMatch(input);
        }
    }
}
