namespace OmniFi_API.Models
{
    public class CryptoHoldings
    {
        public int HoldingID { get; set; }
        public int CryptoExchangeAccountID { get; set; }
        public int CurrencyID { get; set; }
        public decimal Amount { get; set; }
        public decimal Value { get; set; }
    }
}
