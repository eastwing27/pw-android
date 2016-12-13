using System.Security.Cryptography;
using System.Text;

namespace PW.Android
{
    public static class Extender
    {
        /// <summary>
        /// Generate MD5 hash using current string
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static string GetPasswordHash(this string Source)
        {
            var byteArray = Encoding.Unicode.GetBytes(Source);
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(byteArray);
            var hashString = new StringBuilder();
            foreach (var sec in hash)
                hashString.Append(sec.ToString("X2"));
            return hashString.ToString();
        }
    }
}
