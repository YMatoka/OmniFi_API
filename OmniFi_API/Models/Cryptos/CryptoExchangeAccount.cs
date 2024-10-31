using OmniFi_API.Models.Identity;
using OmniFi_API.Services.Interfaces;
using OmniFi_DTOs.Dtos.Cryptos;

namespace OmniFi_API.Models.Cryptos
{
    public class CryptoExchangeAccount
    {
        public int ExchangeAccountID { get; set; }
        public required int CryptoExchangeID { get; set; }
        public CryptoExchange? CryptoExchange { get; set; }
        public required string UserID { get; set; }
        public  ApplicationUser?  User { get; set; }
        public  CryptoApiCredential? CryptoApiCredential { get; set; }


        public async Task<bool?> Equals(CryptoExchangeAccountUpdateDTO cryptoExchangeAccountCreateDTO, IStringEncryptionService stringEncryptionService)
        {
            var aesKey = this.CryptoApiCredential?.AesKey?.Key;
            var aesIV = this.CryptoApiCredential?.AesIV?.IV;

            if (aesKey == null || aesIV == null || this.CryptoApiCredential?.ApiKey == null || this.CryptoApiCredential?.ApiSecret == null)
                return null;

            var actualApiKey = await stringEncryptionService.DecryptAsync(this.CryptoApiCredential.ApiKey, aesKey, aesIV!);
            var actualApiSecret = await stringEncryptionService.DecryptAsync(this.CryptoApiCredential.ApiSecret, aesKey, aesIV!);

            return actualApiKey != cryptoExchangeAccountCreateDTO.ApiKey || actualApiSecret != cryptoExchangeAccountCreateDTO.ApiSecret ?
                true :
                false;
        }
    }
}
