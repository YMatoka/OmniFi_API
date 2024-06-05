namespace OmniFi_API.Dtos.Identity
{
    public class LoginResponseDTO
    {
        public required UserDTO User { get; set; }
        public required string Role { get; set; }
        public required string Token { get; set; }
    }
}
