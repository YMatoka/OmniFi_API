namespace OmniFi_API.Dtos.Identity
{
    public class UserDTO
    {
        public required string ID { get; set; }
        public required string UserName { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
    }
}
