using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OpenlabsInstaller.Encryption
{
    public static class AES
    {
        public static string Encrypt(string input, string key, string iv = "0000000000000000")
        {
            key = MD5Hasher.Hash(key);
            return Convert.ToBase64String(AES.EncryptStringToBytes(input, AES.RawBytesFromString(key), AES.RawBytesFromString(iv)));
        }

        public static string Decrypt(string inputBase64, string key, string iv = "0000000000000000")
        {
            key = MD5Hasher.Hash(key);
            return AES.DecryptStringFromBytes(Convert.FromBase64String(inputBase64), AES.RawBytesFromString(key), AES.RawBytesFromString(iv));
        }

        public static byte[] RawBytesFromString(string input)
        {
            List<byte> list = new List<byte>();
            for (int i = 0; i < input.Length; i++)
            {
                byte b = (byte)((ulong)input[i] & 255UL);
                list.Add(b);
            }
            return list.ToArray();
        }

        public static byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
        {
            if (plainText == null || plainText.Length == 0)
                throw new ArgumentNullException(nameof(plainText));
            if (key == null || key.Length == 0)
                throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length == 0)
                throw new ArgumentNullException(nameof(key));

            byte[] array;
            using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
            {
                rijndaelManaged.Key = key;
                rijndaelManaged.IV = iv;
                ICryptoTransform cryptoTransform = rijndaelManaged.CreateEncryptor(rijndaelManaged.Key, rijndaelManaged.IV);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }
                        array = memoryStream.ToArray();
                    }
                }
            }
            return array;
        }

        public static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            if (cipherText == null || cipherText.Length == 0)
                throw new ArgumentNullException(nameof(cipherText));
            if (key == null || key.Length == 0)
                throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length == 0)
                throw new ArgumentNullException(nameof(key));

            string text = null;
            using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
            {
                rijndaelManaged.Key = key;
                rijndaelManaged.IV = iv;
                ICryptoTransform cryptoTransform = rijndaelManaged.CreateDecryptor(rijndaelManaged.Key, rijndaelManaged.IV);
                using (MemoryStream memoryStream = new MemoryStream(cipherText))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            text = streamReader.ReadToEnd();
                        }
                    }
                }
            }
            return text;
        }

        public const string NULL_IV = "0000000000000000";
    }
}
