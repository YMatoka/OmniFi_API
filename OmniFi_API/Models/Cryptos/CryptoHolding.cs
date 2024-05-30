using OmniFi_API.Models.Assets;

namespace OmniFi_API.Models.Cryptos
{
    public class CryptoHolding
    {
        public required int CrytpoHoldingID { get; set; }
        public required string CryptoCurrencyName { get; set; }
        public required string CryptoCurrencySymbol { get; set; }
        public required decimal Amount { get; set; }
        public required AssetTracking AssetTracking { get; set; }
    }
}
