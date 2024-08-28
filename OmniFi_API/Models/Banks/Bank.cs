using OmniFi_API.Models.Assets;

namespace OmniFi_API.Models.Banks
{
    public class Bank
    {
        public int BankID { get; set; }
        public required string BankName { get; set; }
        public required string ImageUrl { get; set; }
        public ICollection<BankSubAccount>? BankAccounts { get; set; }
        public ICollection<BankAccount>? BankCredentials { get; set; }
        public AssetPlatform? AssetPlatform { get; set; }
    }
}
