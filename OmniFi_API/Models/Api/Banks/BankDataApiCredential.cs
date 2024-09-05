using OmniFi_API.Models.Encryption;

namespace OmniFi_API.Models.Api.Banks
{
    public class BankDataApiCredential
    {
        public int BankDataApiId { get; set; }
        public required string AccessToken { get; set; }
        public decimal AccessExpires { get; set; }
        public required string RefreshToken { get; set; }
        public decimal RefreshExpires { get; set; }
        public DateTime AccessTokenCreatedAt { get; set; }
        public DateTime RefreshokenCreatedAt { get; set; }
        public AesKey? AesKey { get; set; }
        public AesIV? AesIV { get; set; }
    }
}
