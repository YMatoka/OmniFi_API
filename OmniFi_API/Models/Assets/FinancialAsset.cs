using OmniFi_API.Models.Abstracts;
using OmniFi_API.Models.Cryptos;
using OmniFi_API.Models.Currencies;
using OmniFi_API.Models.Identity;

namespace OmniFi_API.Models.Assets
{
    public class FinancialAsset : FinancialEntity
    {
        public ICollection<FinancialAssetHistory>? FinancialAssetsHistory {  get; set; }
        public CryptoHolding? CryptoHolding { get; set; }
        public required DateTime FirstRetrievedAt { get; set; }
        public required DateTime LastUpdatedAt { get; set; }

    }
}
