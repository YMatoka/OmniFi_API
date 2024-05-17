namespace OmniFi_API.Models
{
    public class CryptoExchanges
    {
        public int MyProperty { get; set; }
        public required string ExchangeName { get; set; }
        public byte ExchangeLogo { get; set; }
        public required string APIEndpointSpotBalance { get; set; }
    }
}
