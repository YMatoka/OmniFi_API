using OmniFi_API.Models.Currencies;

namespace OmniFi_API.Dtos.Identity
{
    public class RegisterationRequestDTO
    {
        public required string UserName { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public string? FiatCurrencyCode { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
