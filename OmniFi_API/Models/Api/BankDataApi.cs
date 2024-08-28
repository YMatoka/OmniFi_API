using OmniFi_API.Models.Identity;

namespace OmniFi_API.Models.Api
{
    public class BankDataApiCredential
    {
        public required int Id { get; set; }
        public required string UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public required string AccessToken { get; set; }
        public int AccessExpires { get; set; }
        public DateTime AccessTokenCreatedAt { get; set; }
        public string RefreshToken { get; set; }
        public int RefreshExpires { get; set; }
        public DateTime RefreshTokenCreatedAt { get; set; }
    }
}
