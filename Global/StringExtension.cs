using System;
using System.Text;

namespace UnityExtension
{
    public static class StringExtension
    {
        public static string ToBase64(this string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(bytes);
        }

        public static string FromBase64(this string str)
        {
            var bytes = Convert.FromBase64String(str);
            return Encoding.UTF8.GetString(bytes);
        }

        public static string FirstCharToLowerCase(this string str)
        {
            if (string.IsNullOrEmpty(str) == false && char.IsUpper(str[0]))
            {
                if (str.Length > 1)
                {
                    return char.ToLower(str[0]) + str[1..];
                }
                else
                {
                    return str.ToLower();
                }
            }

            return str;
        }
    }
}