using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.IdentityModel.Tokens;
using OmniFi_API.Dtos.Identity;
using OmniFi_API.Models.Controllers;
using OmniFi_API.Repository.Interfaces;
using OmniFi_API.Utilities;
using System.Net;

namespace OmniFi_API.Controllers
{
    [Route($"api/{ControllerRouteNames.UserController}")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private ApiResponse _apiResponse;
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _apiResponse = new();
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            var loginResponse = await _userRepository.Login(loginRequestDTO);

            if (loginResponse is null || loginResponse.User is null || string.IsNullOrEmpty(loginResponse.Token))
            {
                _apiResponse.IsSucess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add("username or password incorrect");
                return BadRequest(_apiResponse);
            }

            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.IsSucess = true;
            _apiResponse.Result = loginResponse;
            return Ok(_apiResponse);
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Register([FromBody] RegisterationRequestDTO registerationRequestDTO)
        {
            
            if (_userRepository.IsUserExistsByUserName(registerationRequestDTO.UserName))
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSucess = false;
                _apiResponse.ErrorMessages.Add($"username '{registerationRequestDTO.UserName}' already exists");
                return BadRequest(_apiResponse);
            }

            if (_userRepository.IsUserExistsByEmail(registerationRequestDTO.Email))
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSucess = false;
                _apiResponse.ErrorMessages.Add($"email '{registerationRequestDTO.Email}' already exists");
                return BadRequest(_apiResponse);
            }

            var newUser = await _userRepository.Register(registerationRequestDTO);
            
            if (newUser is null)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSucess = false;
                _apiResponse.ErrorMessages.Add("Error while registering");
                return BadRequest(_apiResponse);
            }

            _apiResponse.IsSucess = true;
            _apiResponse.StatusCode = HttpStatusCode.Created;
            _apiResponse.Result = newUser;
            return Ok(_apiResponse);

        }


        


    }
}
