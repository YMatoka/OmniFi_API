using OmniFi_API.Dtos.Identity;

namespace OmniFi_API.Repository.Interfaces
{
    public interface IUserRepository
    {
        public bool IsUniqueUser(string username);
        public Task<UserDTO> Register(RegisterationRequestDTO registerationRequestDTO);
        public Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
    }
}
