using System;
using System.Text;

namespace EsendexApi
{
    public static class BasicAuthenticationToken
    {
        public static string Get(string username, string password)
        {
            var unencodedString = string.Format("{0}:{1}", username, password);

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(unencodedString));
        }
    }
}
