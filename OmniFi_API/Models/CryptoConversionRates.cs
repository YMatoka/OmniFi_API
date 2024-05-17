namespace OmniFi_API.Models
{
    public class CryptoConversionRates
    {
        public int CryptoConversionRateID { get; set; }
        public int SourceCurrencyID { get; set; }
        public int TargetCurrencyID { get; set; }
        public decimal Rate { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
