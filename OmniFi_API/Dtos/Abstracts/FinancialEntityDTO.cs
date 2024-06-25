namespace OmniFi_API.Dtos.Abstracts
{
    public abstract class FinancialEntityDTO
    {
        public int FinancialEntityId { get; set; }
        public required string UserName { get; set; }
        public required string AssetPlatformName { get; set; }
        public required string AssetSourceName { get; set; }
        public required decimal Value { get; set; }
        public required string FiatCurrencyCode { get; set; }
    }
}
