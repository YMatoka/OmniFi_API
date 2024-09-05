using OmniFi_API.Models.Api.Banks;
using OmniFi_API.Models.Assets;

namespace OmniFi_API.Models.Banks
{
    public class Bank
    {
        public int BankID { get; set; }
        public required string BankName { get; set; }
        public required string ImageUrl { get; set; }
        public ICollection<BankAccount>? BankAccounts { get; set; }
        public ICollection<BankAgreement>? BankAgreements { get; set; }
        public AssetPlatform? AssetPlatform { get; set; }
    }
}
