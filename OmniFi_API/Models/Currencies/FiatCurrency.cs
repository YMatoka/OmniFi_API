using OmniFi_API.Models.Assets;
using OmniFi_API.Models.Banks;
using OmniFi_API.Models.Identity;

namespace OmniFi_API.Models.Currencies
{
    public class FiatCurrency
    {
        public int FiatCurrencyID { get; set; }
        public required string CurrencyCode { get; set; }
        public required string CurrencyName { get; set; }
        public required string CurrencySymbol { get; set; }
        public ICollection<ApplicationUser>? Users { get; set; }
        public ICollection<FinancialAsset>? FinancialAssets { get; set; }
        public ICollection<FinancialAssetHistory>? FinancialAssetsHistory { get; set; }

    }
}
