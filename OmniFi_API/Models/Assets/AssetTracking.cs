using OmniFi_API.Models.Cryptos;
using OmniFi_API.Models.Currencies;
using OmniFi_API.Models.Identity;

namespace OmniFi_API.Models.Assets
{
    public class AssetTracking
    {
        public int AssetTrackingID { get; set; }
        public required string UserID { get; set; }
        public required ApplicationUser User { get; set; }
        public int AssetPlatformID { get; set; }
        public required AssetPlatform AssetPlatform { get; set; }
        public int AssetSourceID { get; set; }
        public required AssetSource AssetSource { get; set; }
        public int? CrytpoHoldingID { get; set; }
        public CryptoHolding? CryptoHolding { get; set; }
        public decimal Value { get; set; } 
        public int FiatCurrencyID { get; set; }
        public required FiatCurrency FiatCurrency { get; set; }
        public DateTime RetrievalDate { get; set; }
    }
}
