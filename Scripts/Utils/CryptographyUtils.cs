using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Utils
{
    public static class CryptographyUtils
    {
        public static string Encrypt(string value, string key) => Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(value), key));

        [DebuggerNonUserCode]
        public static string Decrypt(string value, string key)
        {
            string result;

            try
            {
                using CryptoStream cryptoStream = InternalDecrypt(Convert.FromBase64String(value), key);
                using StreamReader streamReader = new StreamReader(cryptoStream);
                result = streamReader.ReadToEnd();
            }
            catch (CryptographicException e)
            {
                UnityEngine.Debug.LogException(e);
                return null;
            }

            return result;
        }

        private static byte[] Encrypt(byte[] key, string value)
        {
            SymmetricAlgorithm symmetricAlgorithm = Rijndael.Create();
            ICryptoTransform cryptoTransform =
                symmetricAlgorithm.CreateEncryptor(new Rfc2898DeriveBytes(value, new byte[16]).GetBytes(16),
                    new byte[16]);

            using MemoryStream memoryStream = new MemoryStream();
            using CryptoStream cryptoStream =
                new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write);
            cryptoStream.Write(key, 0, key.Length);
            cryptoStream.FlushFinalBlock();

            var result = memoryStream.ToArray();

            memoryStream.Close();
            memoryStream.Dispose();

            return result;
        }

        private static CryptoStream InternalDecrypt(byte[] key, string value)
        {
            SymmetricAlgorithm symmetricAlgorithm = Rijndael.Create();
            ICryptoTransform cryptoTransform =
                symmetricAlgorithm.CreateDecryptor(new Rfc2898DeriveBytes(value, new byte[16]).GetBytes(16),
                    new byte[16]);

            MemoryStream memoryStream = new MemoryStream(key);
            return new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Read);
        }

        public static byte[] Base64UrlDecode(string input)
        {
            string output = input;
            output = output.Replace('-', '+');
            output = output.Replace('_', '/');
            switch (output.Length % 4)
            {
                case 0:
                    break;
                case 2:
                    output += "==";
                    break;
                case 3:
                    output += "=";
                    break;
                default:
                    throw new Exception("Illegal base64url string!");
            }

            byte[] converted = Convert.FromBase64String(output);
            return converted;
        }


    }
}