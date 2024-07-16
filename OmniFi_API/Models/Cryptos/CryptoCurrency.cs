namespace OmniFi_API.Models.Cryptos
{
    public class CryptoCurrency
    {
        public int CurrencyID { get; set; }
        public required string CurrencyName { get; set; }
        public required string CurrencySymbol{ get; set; }
        public ICollection<CryptoHolding>? CryptoHoldings { get; set; }
        public ICollection<CryptoHoldingHistory>? CryptoHoldingsHistory { get; set; }
    }
}
