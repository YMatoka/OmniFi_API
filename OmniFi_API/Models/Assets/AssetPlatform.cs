using OmniFi_API.Models.Banks;
using OmniFi_API.Models.Cryptos;

namespace OmniFi_API.Models.Assets
{
    public class AssetPlatform
    {
        public int AssetPlatformID { get; set; }
        public int? BankID { get; set; }
        public Bank? Bank { get; set; }
        public int? CryptoExchangeID { get; set;}
        public CryptoExchange? CryptoExchange { get; set; }
        public ICollection<AssetTracking>? AssetTrackings { get; set; }
    }
}
