namespace OmniFi_API.Dtos.Identity
{
    public class LoginRequestDTO
    {
        public required string UserNameOrEmail { get; set; }
        public required string Password { get; set; }
    }
}
