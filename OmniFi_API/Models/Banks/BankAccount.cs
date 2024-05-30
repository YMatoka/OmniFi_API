using OmniFi_API.Models.Currencies;
using OmniFi_API.Models.Identity;

namespace OmniFi_API.Models.Banks
{
    public class BankAccount
    {
        public int BankAccountID { get; set; }
        public required string UserID { get; set; }
        public required ApplicationUser User { get; set; }
        public int BankID { get; set; }
        public required Bank Bank { get; set; }

    }
}
