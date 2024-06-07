using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OmniFi_API.Data;
using OmniFi_API.Dtos.Identity;
using OmniFi_API.Models.Identity;
using OmniFi_API.Options.Identity;
using OmniFi_API.Repository.Interfaces;
using OmniFi_API.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OmniFi_API.Repository.Identity
{
    public class UserRepository : IUserRepository
    {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly UserRepositoryOptions _options;

        public UserRepository(
            ApplicationDbContext db, 
            UserManager<ApplicationUser> userManager, 
            RoleManager<ApplicationRole> roleManager, 
            IMapper mapper, 
            IOptions<UserRepositoryOptions> options)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _options = options.Value;
        }

        public bool IsUniqueUser(string username)
        {
        
            return _db.Users
                .FirstOrDefault(x => x.UserName == username) is null ?
                true :
                false;
        }

        public async Task<LoginResponseDTO?> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = _db.Users
                .FirstOrDefault(x => x.UserName == loginRequestDTO.UserName);

            if (user == null)
                return null;

            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

            if (isValid == false)
                return null;

            var roles = await _userManager.GetRolesAsync(user);
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_options.SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault()!)
                }),
                Expires = DateTime.UtcNow.AddDays(_options.ExpirationTime),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new LoginResponseDTO()
            {
                User = _mapper.Map<UserDTO>(user),
                Role = roles.FirstOrDefault()!,
                Token = tokenHandler.WriteToken(token)
            };
        }

        public async Task<UserDTO?> Register(RegisterationRequestDTO registerationRequestDTO)
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
                    if (!(_roleManager.RoleExistsAsync(Roles.Admin).GetAwaiter().GetResult()))
                    {
                        await _roleManager.CreateAsync(new ApplicationRole() { Name = Roles.Admin });
                    }

                    if (!(_roleManager.RoleExistsAsync(Roles.User).GetAwaiter().GetResult()))
                    {
                        await _roleManager.CreateAsync(new ApplicationRole() { Name = Roles.User });
                    }

                    await _userManager.AddToRoleAsync(user, Roles.User);

                    return _mapper.Map<UserDTO>(_db.Users
                        .FirstOrDefault(x => x.UserName == user.UserName));
                }

            }
            catch (Exception)
            {

                throw;
            }

            return null;

        }
    }
}
