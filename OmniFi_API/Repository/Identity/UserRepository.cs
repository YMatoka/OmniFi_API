﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OmniFi_API.Data;
using OmniFi_API.Data.Interfaces;
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

        private readonly IApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly UserRepositoryOptions _options;

        public UserRepository(
            IApplicationDbContext db,
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

            if (loginRequestDTO.UserName is not null)
            {
                user = _db.Users
                    .FirstOrDefault(x => (x.UserName == loginRequestDTO.UserName));
            }
            else if (loginRequestDTO.Email is not null)
            {
                user = _db.Users
                    .FirstOrDefault(x => (x.NormalizedEmail == loginRequestDTO.Email.ToUpper()));
            }

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

                    foreach(var error in result.Errors)
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

            return null;

        }
    }
}
