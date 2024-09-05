using OmniFi_API.Models.Currencies;
using OmniFi_API.Models.Identity;

namespace OmniFi_API.Models.Banks
{
    public class BankSubAccount
    {
        public required string BankSubAccountID { get; set; }
        public required int BankAccountID { get; set; }
        public BankAccount? BankAccount { get; set; }
    }
}
