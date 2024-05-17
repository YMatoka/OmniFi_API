namespace OmniFi_API.Models
{
    public class BankAccounts
    {
        public int BankAccountID { get; set; }
        public int UserID { get; set; }
        public required string AccountType { get; set; }
        public int AccoutnNumber { get; set; }
        public decimal Balance { get; set; }
        public int CurrencyID { get; set; }
        public string APIKey { get; set; }
        public string APISecret { get; set; }
    }
}
