using OmniFi_API.Models.Encryption;
using OmniFi_API.Models.Identity;
using OmniFi_API.Models.Banks;

namespace OmniFi_API.Models.Banks
{
    public class BankAccount
    {
        public int BankAccountId { get; set; }
        public required string UserID { get; set; }
        public ApplicationUser? User { get; set; }
        public required string RequisitionId { get; set; }
        public DateTime RequisitionCreatedAt  { get; set; }
        public bool IsAccessGranted { get; set; } = false;
        public double AccessDurationInDays { get; set; }
        public DateTime AccessGrantedAt { get; set; }
        public ICollection<BankSubAccount>? BankSubAccounts { get; set; }
        public required int BankId { get; set; }
        public Bank? Bank { get; set; }

    }
}