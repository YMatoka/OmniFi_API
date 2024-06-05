using Microsoft.AspNetCore.Identity;
using OmniFi_API.Data;
using OmniFi_API.Dtos.Identity;
using OmniFi_API.Models.Identity;
using OmniFi_API.Repository.Interfaces;
using OmniFi_API.Utilities;

namespace OmniFi_API.Repository.Identity
{
    public class UserRepository : IUserRepository
    {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public bool IsUniqueUser(string username)
        {
            return _db.Users
                .FirstOrDefault(x => x.UserName == username) is null ?
                true :
                false;
        }

        public Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            throw new NotImplementedException();
        }

        public async Task<UserDTO> Register(RegisterationRequestDTO registerationRequestDTO)
        {

            var currency = _db.FiatCurrencies.FirstOrDefault(x => x.CurrencyCode == registerationRequestDTO.FiatCurrencyCode) ??
                _db.FiatCurrencies.First(x => x.CurrencyCode == FiatCurrencyCodes.EUR);

            ApplicationUser user = new()
            {
                UserName = registerationRequestDTO.UserName,
                FirstName = registerationRequestDTO.FirstName,
                LastName = registerationRequestDTO.LastName,
                Email = registerationRequestDTO.Email,
                NormalizedEmail = registerationRequestDTO.Email.ToUpper(),
                FiatCurrency = currency
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registerationRequestDTO.Password);

                if(result.Succeeded)
                {

                }

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
