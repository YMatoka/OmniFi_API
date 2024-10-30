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
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace OmniFi_API.Controllers.Identity
{
    [Route($"api/{ControllerRouteNames.UserController}")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private ApiResponse _apiResponse;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserController> _logger;

        public UserController(
            IUserRepository userRepository, 
            ILogger<UserController> logger)
        {
            _userRepository = userRepository;
            _apiResponse = new();
            _logger = logger;
        }

        [HttpPost(nameof(Login))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
                _logger.LogError(ex, ErrorMessages.ErrorPostMethodMessage
                    .Replace(ErrorMessages.VariableTag, nameof(Login)));
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.AddErrorMessage(ErrorMessages.Error500Message);
                return StatusCode(500, _apiResponse);
            }

        }

        [HttpPut(nameof(Put))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status304NotModified)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> Put(
            [Required] string usernameOrEmail,
            [FromBody] UserUpdateDTO userUpdateDTO
        )
            {

            }

        [HttpPost(nameof(Register))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

                    var errorMessages = ErrorMessages.ErrorPostMethodMessage.Replace(ErrorMessages.VariableTag, nameof(Register))  + " : "
                        + Environment.NewLine
                        +  string.Join(Environment.NewLine, response.ErrorMessages);

                    _logger.LogWarning(errorMessages);
                    _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                    _apiResponse.IsSuccess = false;
                    _apiResponse.AddErrorMessage(ErrorMessages.Error500Message);
                    return StatusCode(500, _apiResponse);
                }

                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode = HttpStatusCode.Created;
                _apiResponse.Result = response.User;
                return CreatedAtAction(nameof(Register), _apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorMessages.ErrorPostMethodMessage
                    .Replace(ErrorMessages.VariableTag, nameof(Register)));
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.AddErrorMessage(ErrorMessages.Error500Message);
                return StatusCode(500, _apiResponse);
            }

        }

        [HttpDelete(nameof(Delete))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<ApiResponse>> Delete([Required] string usernameOrEmail)
        {
            try
            {
                var user = await _userRepository.GetUserAsync(usernameOrEmail);

                if (user is null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.AddErrorMessage(ErrorMessages.ErrorUserNotFoundMessage
                        .Replace(ErrorMessages.VariableTag, usernameOrEmail));
                    return NotFound(_apiResponse);
                }

                await _userRepository.RemoveAsync(user);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorMessages.ErrorDeleteMethodMessage
                    .Replace(ErrorMessages.VariableTag, nameof(Delete)));
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.AddErrorMessage(ErrorMessages.Error500Message);
                return StatusCode(500, _apiResponse);
            }
        }

    }
}
