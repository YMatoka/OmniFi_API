using OmniFi_DTOs.Dtos.Identity;
using OmniFi_API.Models.Identity;

namespace OmniFi_API.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetUserAsync(string usernameOrEmail, bool tracked = false);
        Task<ApplicationUser?> UpdateAsync(ApplicationUser user, UserUpdateDTO userUpdateDTO);
        Task RemoveAsync(ApplicationUser user);
        Task<ApplicationUser?> GetWithAllAccountsAsync(string usernameOrEmail, bool tracked = false);
        bool IsUserExistsByEmail(string email);
        bool IsUserExistsByUserName(string username);
        Task<LoginResponseDTO?> Login(LoginRequestDTO loginRequestDTO);
        Task<IdentityResponse> Register(RegisterationRequestDTO registerationRequestDTO);
    }
}