using OmniFi_API.Models.Encryption;
using OmniFi_API.Models.Identity;

namespace OmniFi_API.Models.Banks
{
    public class BankAccount
    {
        public int BankAccountID { get; set; }
        public required string UserID { get; set; }
        public ApplicationUser? User { get; set; }
        public required string RequisitionId { get; set; }
        public required int BankSubAccountID { get; set; }
        public ICollection<BankSubAccount>? BankSubAccounts { get; set; }

    }
}