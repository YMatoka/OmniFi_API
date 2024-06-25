using Microsoft.Extensions.Options;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using OmniFi_API.Data;
using OmniFi_API.Models.Cryptos;
using OmniFi_API.Repository.Interfaces;
using OmniFi_API.Services.Interfaces;
using System.Linq.Expressions;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace OmniFi_API.Repository.Cryptos
{
    public class CryptoApiCredentialRepository : BaseRepository<CryptoApiCredential>, ICryptoApiCredentialRepository
    {
        private readonly IStringEncryptionService _stringEncryptionService;
        const int IvVectorByteSize = 16;
        public CryptoApiCredentialRepository(
            ApplicationDbContext db, 
            IStringEncryptionService stringEncryptionService) : base(db)
        {
            _stringEncryptionService = stringEncryptionService;
        }

        public override async Task<CryptoApiCredential?> GetAsync(Expression<Func<CryptoApiCredential, bool>>? filter = null, string? includeProperties = null, bool tracked = true)
        {
            var cryptoApiCredential = await base.GetAsync(filter, includeProperties, tracked);

            //if (cryptoApiCredential is not null)
            //{
            //    var encryptedKey = await _stringEncryptionService.EncryptAsync(cryptoApiCredential.ApiKey, _options.ApiKeyEncryptionKey);
            //    var encryptedSecretKey = await _stringEncryptionService.EncryptAsync(cryptoApiCredential.ApiSecret, _options.ApiSecretEncryptionKey);

            //    var decryptedKey = await _stringEncryptionService.DecryptAsync(encryptedKey, _options.ApiKeyEncryptionKey);
            //    var decryptedSecretKey = await _stringEncryptionService.DecryptAsync(encryptedSecretKey, _options.ApiSecretEncryptionKey);

            //}

            return cryptoApiCredential;
        }

        public async Task UpdateAsync(CryptoApiCredential cryptoApiCredential)
        {
            db.Update(cryptoApiCredential);
            await SaveAsync();
        }

        private string EncryptKey(string keyToEncrypt, string encryptionKey)
        {
            using(Aes aes = Aes.Create())
            {
                //aes.GenerateKey();
                aes.Key = Convert.FromHexString(encryptionKey);
                aes.IV = new byte[IvVectorByteSize];
                aes.GenerateIV();

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream  ms = new MemoryStream())
                {
                    //write at the beginning of the memory stream the IV vector in order to retrieve it for decryption
                    ms.Write(aes.IV, 0, aes.IV.Length);

                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using(StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(keyToEncrypt);
                        }
                    }

                    return Convert.ToBase64String(ms.ToArray());
                    var cipherArray = ms.ToArray();
                    var combinedArray = new byte[aes.IV.Length + cipherArray.Length];

                    Array.Copy(aes.IV, 0, combinedArray, 0, aes.IV.Length);
                    Array.Copy(cipherArray, 0, combinedArray, aes.IV.Length, cipherArray.Length);
                    return Convert.ToBase64String(combinedArray);
                }
            }
        }

        private string DecryptKey(string keyToDecrypt, string encryptionKey)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Convert.FromHexString(encryptionKey);

                var fullCipher = Convert.FromBase64String(keyToDecrypt);

                byte[] iv = new byte[IvVectorByteSize];
                //byte[] cipher = new byte[fullCipher.Length - IvVectorByteSize];

                //// retrieve the iv vector from the first bytes of the cipher
                //Array.Copy(fullCipher, 0, iv, 0, iv.Length);
                //Array.Copy(fullCipher, iv.Length, cipher, 0, cipher.Length);

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, iv);

                using (MemoryStream ms = new MemoryStream(fullCipher))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
 