using Microsoft.AspNetCore.Identity;
using OmniFi_API.Models.Assets;
using OmniFi_API.Models.Banks;
using OmniFi_API.Models.Cryptos;
using OmniFi_API.Models.Currencies;
using OmniFi_API.Utilities;

namespace OmniFi_API.Models.Identity
{
    public class ApplicationUser : IdentityUser<string>
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<BankAccount>? BankAccounts { get; set; }
        public ICollection<CryptoExchangeAccount>? CryptoExchangeAccounts { get; set; }
        public ICollection<CryptoApiCredential>? ApiCredentials { get; set; }
        public ICollection<BankCredential>? BankCredentials { get; set; }
        public ICollection<AssetTracking>? AssetTrackings { get; set; }
        public int SelectedFiatCurrencyID { get; set; }
        public  required FiatCurrency FiatCurrency { get; set; }

    }
}
