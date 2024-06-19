using OmniFi_API.Models.Encryption;
using OmniFi_API.Models.Identity;

namespace OmniFi_API.Models.Cryptos
{
    public class CryptoApiCredential
    {
        public int CryptoApiCredentialID { get; set; }
        public required byte[] ApiKey { get; set; }
        public required byte[] ApiSecret { get; set; }
        public required int CryptoExchangeAccountID { get; set; }
        public CryptoExchangeAccount? CryptoExchangeAccount { get; set; }
        public AesKey? AesKey { get; set; }
    }
}
