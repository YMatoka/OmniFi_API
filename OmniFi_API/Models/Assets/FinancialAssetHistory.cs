using OmniFi_API.Models.Abstracts;
using OmniFi_API.Models.Cryptos;

namespace OmniFi_API.Models.Assets
{
    public class FinancialAssetHistory : FinancialEntity
    {
        public required int FinancialAssetId {  get; set; }
        public FinancialAsset? FinancialAsset { get; set; }
        public CryptoHoldingHistory? CryptoHoldingHistory { get; set; }
        public required DateTime RecordedAt { get; set; }
    }
}
