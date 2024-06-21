namespace OmniFi_API.Dtos.Banks
{
    public class BankAccountCreateDTO
    {
        public required string UsernameOrEmail { get; set; }
        public required string BankName { get; set; }
        public required string Password { get; set; }
    }
}
