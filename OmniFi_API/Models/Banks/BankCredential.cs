using OmniFi_API.Models.Identity;

namespace OmniFi_API.Models.Banks
{
    public class BankCredential
    {
        public int BankCredientialID { get; set; }
        public int BankUserID { get; set; }
        public required string Password { get; set; }
        public int BankID { get; set; }
        public required Bank Bank { get; set; }
        public required string UserID { get; set; }
        public required ApplicationUser ApplicationUser { get; set; }
    }
}