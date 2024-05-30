using OmniFi_API.Models.Assets;

namespace OmniFi_API.Models.Cryptos

{
    public class CryptoExchange
    {
        public int CryptoExchangeID { get; set; }
        public required string ExchangeName { get; set; }
        public byte[]? ExchangeLogo { get; set; }
        public ICollection<CryptoExchangeAccount>? cryptoExchangeAccounts { get; set; }
        public ICollection<CryptoApiCredential>? ApiCredentials { get; set; }
        public AssetPlatform? AssetPlatform { get; set; }
    }
}
