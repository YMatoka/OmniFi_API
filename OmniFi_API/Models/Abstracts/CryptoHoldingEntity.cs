using OmniFi_API.Models.Assets;

namespace OmniFi_API.Models.Abstracts
{
    public abstract class CryptoHoldingEntity
    {
        public required int CryptoHoldingEntityId { get; set; }
        public required string CryptoCurrencyName { get; set; }
        public required string CryptoCurrencySymbol { get; set; }
        public required decimal Quantity { get; set; }

    }
}
