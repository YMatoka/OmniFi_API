using OmniFi_API.Models.Identity;

namespace OmniFi_API.Models.Cryptos
{
    public class CryptoExchangeAccount
    {
        public int ExchangeAccountID { get; set; }
        public int CryptoExchangeID { get; set; }
        public required CryptoExchange CryptoExchange { get; set; }
        public required string UserID { get; set; }
        public required ApplicationUser User { get; set; }
   
    }
}
