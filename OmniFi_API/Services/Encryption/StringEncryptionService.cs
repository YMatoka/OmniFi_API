using Microsoft.Extensions.Options;
using OmniFi_API.Services.Interfaces;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace OmniFi_API.Services.Encryption
{
    public class StringEncryptionService : IStringEncryptionService
    {

        public async Task<string> DecryptAsync(byte[] encrypted, byte[] encryptionKey, byte[] IV)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = encryptionKey;
                aes.IV = IV;

                using (MemoryStream inputMemoryStream = new MemoryStream(encrypted))
                {
                    using (CryptoStream cryptostream = new CryptoStream(inputMemoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        using (MemoryStream outputMemoryStream = new MemoryStream())
                        {
                            await cryptostream.CopyToAsync(outputMemoryStream);

                            return Encoding.Unicode.GetString(outputMemoryStream.ToArray());
                        }
                    }
                }
            }
        }

        public async Task<byte[]> EncryptAsync(string clearText, byte[] encryptionKey, byte[] IV)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = encryptionKey;
                aes.IV = IV;

                using (MemoryStream outputMemoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptostream = new CryptoStream(outputMemoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        await cryptostream.WriteAsync(Encoding.Unicode.GetBytes(clearText));
                        await cryptostream.FlushFinalBlockAsync();
                    }

                    return outputMemoryStream.ToArray();
                }
            }
        }

        public byte[] GenerateAesIV()
        {
            using(Aes aes = Aes.Create())
            {
                return aes.IV;
            }
        }

        public byte[] GenerateAesKey()
        {
            using(Aes  aes = Aes.Create())
            {
                return aes.Key;
            }
        }
    }
}
