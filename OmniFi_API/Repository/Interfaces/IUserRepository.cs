using OmniFi_API.Dtos.Identity;

namespace OmniFi_API.Repository.Interfaces
{
    public interface IUserRepository
    {
        bool IsUserExistsByEmail(string email);
        bool IsUserExistsByUserName(string username);
        Task<LoginResponseDTO?> Login(LoginRequestDTO loginRequestDTO);
        Task<UserDTO?> Register(RegisterationRequestDTO registerationRequestDTO);
    }
}