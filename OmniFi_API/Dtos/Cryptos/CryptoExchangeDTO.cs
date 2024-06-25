namespace OmniFi_API.Dtos.Cryptos
{
    public class CryptoExchangeDTO
    {
        public int CryptoExchangeID { get; set; }
        public required string ExchangeName { get; set; }
        public required string ImageUrl { get; set; }
    }
}
