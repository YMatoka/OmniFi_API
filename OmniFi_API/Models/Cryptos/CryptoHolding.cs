using OmniFi_API.Models.Abstracts;
using OmniFi_API.Models.Assets;

namespace OmniFi_API.Models.Cryptos
{
    public class CryptoHolding : CryptoHoldingEntity
    {
        public required int FinancialAssetID { get; set; }
        public FinancialAsset? FinancialAsset { get; set; }
        public ICollection<CryptoHoldingHistory>? CryptoHoldingsHistory { get; set; }
    }
}
