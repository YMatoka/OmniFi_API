using OmniFi_API.Models.Encryption;
using OmniFi_API.Models.Identity;

namespace OmniFi_API.Models.Banks
{
    public class BankCredential
    {
        public int BankCredientialID { get; set; }
        public int BankUserID { get; set; }
        public required string Password { get; set; }
        public int BankAccountID { get; set; }
        public BankAccount? BankAccount { get; set; }
        public AesKey? AesKey { get; set; }
    }
}