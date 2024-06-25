using OmniFi_API.Dtos.Abstracts;
using OmniFi_API.Dtos.Cryptos;
using OmniFi_API.Models.Cryptos;

namespace OmniFi_API.Dtos.Assets
{
    public class FinancialAssetDTO : FinancialEntityDTO
    {
        public CryptoHoldingDTO? CryptoHolding { get; set; }
        public DateTime FirstRetrievedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}
