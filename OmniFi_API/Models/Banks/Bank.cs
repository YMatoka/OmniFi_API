using OmniFi_API.Models.Assets;

namespace OmniFi_API.Models.Banks
{
    public class Bank
    {
        public int BankID { get; set; }
        public required string BankName { get; set; }
        public byte[]? BankLogo { get; set; }
        public ICollection<BankAccount>? BankAccounts { get; set; }
        public ICollection<BankCredential>? BankCredentials { get; set; }
        public AssetPlatform? AssetPlatform { get; set; }
    }
}
