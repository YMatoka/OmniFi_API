using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OmniFi_API.Data;
using OmniFi_API.Data.Interfaces;
using OmniFi_DTOs.Dtos.Identity;
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

        public bool IsUserExistsByUserName(string username)
        {

            return _db.Users
                .FirstOrDefault(x => x.UserName == username) is null ?
                false :
                true;
        }

        public bool IsUserExistsByEmail(string email)
        {

            return _db.Users
                .FirstOrDefault(x => x.NormalizedEmail == email.ToUpper()) is null ?
                false :
                true;
        }


        public async Task<LoginResponseDTO?> Login(LoginRequestDTO loginRequestDTO)
        {
            ApplicationUser? user = null;

            if (loginRequestDTO.UserNameOrEmail is not null)
                user = await GetUserAsync(loginRequestDTO.UserNameOrEmail);

            if (user is null)
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

        public async Task<ApplicationUser?> GetUserAsync(string usernameOrEmail, bool tracked = false)
        {
            IQueryable<ApplicationUser> query = _db.Users;

            if (!tracked)
            {
                query.AsNoTracking();
            }

            return await query
                .FirstOrDefaultAsync(x =>
                (x.UserName == usernameOrEmail || x.NormalizedEmail == usernameOrEmail.ToUpper()));
        }

        public async Task<IdentityResponse> Register(RegisterationRequestDTO registerationRequestDTO)
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
                CreatedAt = DateTime.UtcNow,
                FiatCurrency = currency
            };

            IdentityResponse response = new IdentityResponse();

            try
            {
                var result = await _userManager.CreateAsync(user, registerationRequestDTO.Password);

                if (result.Succeeded)
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

                    response.User = _mapper.Map<UserDTO>(_db.Users
                        .FirstOrDefault(x => x.UserName == user.UserName));

                    return response;
                }
                else if (!result.Succeeded)
                {
                    response.IsSucceeded = false;

                    foreach (var error in result.Errors)
                    {
                        response.ErrorMessages.Add(error.Description);
                    }

                    return response;
                }

            }
            catch (Exception)
            {

                throw;
            }

            response.IsSucceeded = false;
            return response;

        }

        public async Task<ApplicationUser?> GetWithAllAccountsAsync(string usernameOrEmail, bool tracked = false)
        {
            IQueryable<ApplicationUser> query = _db.Users;

            if (!tracked)
            {
                query.AsNoTracking();
            }

            query = query
                .Where(x =>
                (x.UserName == usernameOrEmail || x.NormalizedEmail == usernameOrEmail.ToUpper()));

            await query
                .Include(x => x.BankAccounts)
                .Include(x => x.CryptoExchangeAccounts)
                .LoadAsync();

            return await query.FirstOrDefaultAsync();

        }

        public async Task RemoveAsync(ApplicationUser user)
        {
            _db.Remove(user);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(ApplicationUser user, UserUpdateDTO userUpdateDTO)
        {
            if(userUpdateDTO.FirstName is not null)
                user.FirstName = userUpdateDTO.FirstName;

            if (userUpdateDTO.LastName is not null)
                user.LastName = userUpdateDTO.LastName;

            if (userUpdateDTO.Username is not null)
                user.UserName = userUpdateDTO.Username;

            if (userUpdateDTO.FiatCurrencyCode is not null)
            {
                var currency = _db.FiatCurrencies.FirstOrDefault(x => x.CurrencyCode == userUpdateDTO.FiatCurrencyCode) ??
                    _db.FiatCurrencies.First(x => x.CurrencyCode == userUpdateDTO.FiatCurrencyCode);

                if(currency is not null)
                {
                    user.FiatCurrency = currency;
                }
            }
            
            _db.Update(user);
            await _db.SaveChangesAsync();
        }
    }
}
