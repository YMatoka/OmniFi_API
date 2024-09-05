namespace OmniFi_API.Models.Portfolio
{
    public class PortfolioData
    {
        public required string AssetSourceName { get; set; }
        public required string AssetPlatformName { get; set; }
        public required decimal Balance { get; set; }
        public required string FiatCurrencyCode { get; set; }
        public string? CryptoCurrencySymbol { get; set; }
        public decimal? Quantity { get; set; }
    }
}
