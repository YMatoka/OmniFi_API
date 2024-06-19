using OmniFi_API.Dtos.Identity;
using OmniFi_API.Models.Identity;

namespace OmniFi_API.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetUserAsync(string usernameOrEmail, bool tracked = false);
        bool IsUserExistsByEmail(string email);
        bool IsUserExistsByUserName(string username);
        Task<LoginResponseDTO?> Login(LoginRequestDTO loginRequestDTO);
        Task<IdentityResponse> Register(RegisterationRequestDTO registerationRequestDTO);
    }
}