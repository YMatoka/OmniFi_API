using OmniFi_API.Models.Banks;
using OmniFi_API.Models.Identity;

namespace OmniFi_API.Models.Api.Banks
{
    public class BankAgreement
    {
        public int BankAgreementId { get; set; }
        public required string UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public required string BankInstitutionId { get; set; }
        public required int BankId { get; set; }
        public Bank? Bank { get; set; }
        public required string[] AccessScope { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
