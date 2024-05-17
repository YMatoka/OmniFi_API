namespace OmniFi_API.Models
{
    public class FiatCurrencies
    {
        public int CurrencyID { get; set; }
        public required string CurrencyCode { get; set; }
        public required string CurrencyName { get; set; }
    }
}
