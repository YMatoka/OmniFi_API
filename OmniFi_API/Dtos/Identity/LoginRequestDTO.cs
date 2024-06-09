namespace OmniFi_API.Dtos.Identity
{
    public class LoginRequestDTO
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public required string Password { get; set; }
    }
}
