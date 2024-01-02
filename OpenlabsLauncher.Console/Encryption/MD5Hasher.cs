using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OpenlabsInstaller.Encryption
{
    internal static class MD5Hasher
    {
        internal static string Hash(string input)
        {
            string text;
            using (MD5 md = MD5.Create())
            {
                byte[] bytes = Encoding.ASCII.GetBytes(input);
                byte[] array = md.ComputeHash(bytes);
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < array.Length; i++)
                {
                    stringBuilder.Append(array[i].ToString("X2"));
                }
                text = stringBuilder.ToString().ToLower();
            }
            return text;
        }
    }
}
