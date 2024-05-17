using Microsoft.AspNetCore.Identity;

namespace OmniFi_API.Models
{
    public class Users: IdentityUser
    {
        public int UserID { get; set; }
        public override required string UserName { get; set; }
        public required string Name { get; set; }
        public override required string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
