using OmniFi_API.Models.Abstracts;
using OmniFi_API.Models.Assets;

namespace OmniFi_API.Models.Cryptos
{
    public class CryptoHoldingHistory : CryptoHoldingEntity
    {
        public required int FinancialAssetHistoryID { get; set; }
        public FinancialAssetHistory? FinancialAssetHistory { get; set; }
        public required int CryptoHoldingId { get; set; }
        public CryptoHolding? CryptoHolding { get; set; }
    }
}
