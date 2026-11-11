using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
namespace btl_myselt.Helpers
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes); // Trả về chuỗi ký tự in hoa 64 ký tự
        }
        
        public static bool VerifyPassword(string inputPassword, string storedHash)
        {
            return HashPassword(inputPassword).Equals(storedHash, StringComparison.OrdinalIgnoreCase);
        }
    }
}