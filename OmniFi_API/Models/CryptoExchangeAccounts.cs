namespace OmniFi_API.Models
{
    public class CryptoExchangeAccounts
    {
        public int ExchangeAccountID { get; set; }
        public int ExchangeID { get; set; }
        public int UserID { get; set; }
        public decimal Balance { get;set; }
        public required string APIKey { get; set; }
        public required string APISecret { get; set; }
    }
}
