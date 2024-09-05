using OmniFi_API.Models.Api.Banks;
using OmniFi_API.Models.Banks;
using OmniFi_API.Models.Cryptos;

namespace OmniFi_API.Models.Encryption
{
    public class AesIV
    {
        public int AesIVId { get; set; }
        public required byte[] IV { get; set; }
        public int? BankDataApiCredentialId { get; set; }
        public BankDataApiCredential? BankDataApiCredential { get; set; }
        public int? CryptoApiCredentialId { get; set; }
        public CryptoApiCredential? CryptoApiCredential { get; set; }
    }
}
