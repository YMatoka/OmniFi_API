using OmniFi_API.Models.Identity;

namespace OmniFi_API.Models.Cryptos
{
    public class CryptoApiCredential
    {
        public int ApiCrendentialsID { get; set; }
        public required string ApiKey { get; set; }
        public required string ApiSecret { get; set; }
        public required string UserId { get; set; }
        public required ApplicationUser ApplicationUser { get; set; }
        public int CryptoExchangeID { get; set; }
        public required CryptoExchange CryptoExchange { get; set; }
    }
}
