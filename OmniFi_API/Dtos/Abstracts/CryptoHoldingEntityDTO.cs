namespace OmniFi_API.Dtos.Abstracts
{
    public abstract class CryptoHoldingEntityDTO
    {
        public required int CryptoHoldingEntityId { get; set; }
        public required string CryptoCurrencyName { get; set; }
        public required string CryptoCurrencySymbol { get; set; }
        public required decimal Amount { get; set; }
    }
}
