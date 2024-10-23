using OmniFi_API.Models.Assets;
using OmniFi_API.Models.Cryptos;
using OmniFi_API.Models.Currencies;
using OmniFi_API.Models.Identity;

namespace OmniFi_API.Models.Abstracts
{
    public abstract class FinancialEntity
    {
        public int FinancialEntityId { get; set; }
        public required string UserID { get; set; }
        public ApplicationUser? User { get; set; }
        public required int AssetPlatformID { get; set; }
        public AssetPlatform? AssetPlatform { get; set; }
        public required int AssetSourceID { get; set; }
        public AssetSource? AssetSource { get; set; }
        public required decimal Amount { get; set; }
        public required int FiatCurrencyID { get; set; }
        public FiatCurrency? FiatCurrency { get; set; }
    }
}
