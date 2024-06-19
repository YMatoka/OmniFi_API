using OmniFi_API.Models.Identity;

namespace OmniFi_API.Models.Cryptos
{
    public class CryptoExchangeAccount
    {
        public int ExchangeAccountID { get; set; }
        public required int CryptoExchangeID { get; set; }
        public CryptoExchange? CryptoExchange { get; set; }
        public required string UserID { get; set; }
        public  ApplicationUser?  User { get; set; }
        public  CryptoApiCredential? CryptoApiCredential { get; set; }
   
    }
}
