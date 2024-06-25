namespace OmniFi_API.Dtos.Cryptos
{
    public class CryptoExchangeAccountCreateDTO
    {
        public required string UsernameOrEmail { get; set; }
        public required string CryptoExchangeName { get; set; }
        public required string ApiKey { get; set; } 
        public required string ApiSecret { get; set;}

    }
}
