using OmniFi_API.Models.Currencies;
using OmniFi_API.Models.Identity;

namespace OmniFi_API.Models.Banks
{
    public class BankAccount
    {
        public int BankAccountID { get; set; }
        public required string UserID { get; set; }
        public ApplicationUser? User { get; set; }
        public required int BankID { get; set; }
        public Bank? Bank { get; set; }
        public BankCredential? BankCredential { get; set; }

    }
}
