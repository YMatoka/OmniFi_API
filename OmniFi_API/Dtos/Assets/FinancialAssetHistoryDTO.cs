using OmniFi_API.Dtos.Abstracts;
using OmniFi_API.Dtos.Cryptos;

namespace OmniFi_API.Dtos.Assets
{
    public class FinancialAssetHistoryDTO : FinancialEntityDTO
    {
        public int FinancialAssetId { get; set; }
        public CryptoHoldingHistoryDTO? CryptoHoldingHistory { get; set; }
        public DateTime RecordedAt { get; set; }
    }
}
