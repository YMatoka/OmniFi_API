using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.IdentityModel.Tokens;
using OmniFi_DTOs.Dtos.Identity;
using OmniFi_DTOs.Dtos.Api;
using OmniFi_API.Repository.Interfaces;
using OmniFi_API.Utilities;
using System.Net;

namespace OmniFi_API.Controllers.Identity
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

        [HttpPost(nameof(Login))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            try
            {
                var loginResponse = await _userRepository.Login(loginRequestDTO);

                if (loginResponse is null || loginResponse.User is null || string.IsNullOrEmpty(loginResponse.Token))
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.AddErrorMessage($"Invalid credentials\n\n" +
                        $"Your entered credentials may be incorrect or email sign-in is disabled for your account.\n\n" +
                        $"Please check your credentials and try again.");
                    return BadRequest(_apiResponse);
                }

                _apiResponse.StatusCode = HttpStatusCode.OK;
                _apiResponse.IsSuccess = true;
                _apiResponse.Result = loginResponse;
                return Ok(_apiResponse);

            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.AddErrorMessage(ex.Message);
                return _apiResponse;
            }

        }

        [HttpPost(nameof(Register))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse>> Register([FromBody] RegisterationRequestDTO registerationRequestDTO)
        {
            try
            {
                if (_userRepository.IsUserExistsByUserName(registerationRequestDTO.UserName))
                {
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.IsSuccess = false;
                    _apiResponse.AddErrorMessage($"username '{registerationRequestDTO.UserName}' already exists");
                    return BadRequest(_apiResponse);
                }

                if (_userRepository.IsUserExistsByEmail(registerationRequestDTO.Email))
                {
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.IsSuccess = false;
                    _apiResponse.AddErrorMessage($"email '{registerationRequestDTO.Email}' already exists");
                    return BadRequest(_apiResponse);
                }

                var response = await _userRepository.Register(registerationRequestDTO);

                if (!response.IsSucceeded)
                {
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages = response.ErrorMessages;
                    return BadRequest(_apiResponse);
                }

                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode = HttpStatusCode.Created;
                _apiResponse.Result = response.User;
                return CreatedAtAction(nameof(Register), _apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.AddErrorMessage(ex.Message);
                return _apiResponse;
            }

        }

    }
}
