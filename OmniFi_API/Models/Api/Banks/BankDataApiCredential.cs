using OmniFi_API.Models.Encryption;

namespace OmniFi_API.Models.Api.Banks
{
    public class BankDataApiCredential
    {
        public int BankDataApiId { get; set; }
        public required string AccessToken { get; set; }
        public double AccessExpires { get; set; }
        public required string RefreshToken { get; set; }
        public double RefreshExpires { get; set; }
        public DateTime AccessTokenCreatedAt { get; set; }
        public DateTime RefreshokenCreatedAt { get; set; }
    }
}
