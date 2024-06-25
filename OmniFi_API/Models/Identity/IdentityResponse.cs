using OmniFi_DTOs.Dtos.Identity;

namespace OmniFi_API.Models.Identity
{
    public class IdentityResponse
    {
        public bool IsSucceeded { get; set; } = true;
        public UserDTO? User { get; set; } = null;
        public List<string> ErrorMessages { get; set;} = new List<string>();
    }
}
