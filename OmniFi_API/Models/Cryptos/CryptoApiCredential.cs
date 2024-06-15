using OmniFi_API.Models.Identity;

namespace OmniFi_API.Models.Cryptos
{
    public class CryptoApiCredential
    {
        public int CryptoApiCredentialID { get; set; }
        public required string ApiKey { get; set; }
        public required string ApiSecret { get; set; }
        public int CryptoExchangeAccountID { get; set; }
        public required CryptoExchangeAccount CryptoExchangeAccount { get; set; }
    }
}
