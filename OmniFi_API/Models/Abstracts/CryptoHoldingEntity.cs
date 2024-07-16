using OmniFi_API.Models.Assets;
using OmniFi_API.Models.Cryptos;

namespace OmniFi_API.Models.Abstracts
{
    public abstract class CryptoHoldingEntity
    {
        public int CryptoHoldingEntityId { get; set; }
        public required int CryptoCurrencId { get; set; }
        public CryptoCurrency? CryptoCurrency{ get; set; }
        public required decimal Quantity { get; set; }

    }
}
