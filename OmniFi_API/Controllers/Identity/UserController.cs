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
using OmniFi_API.Models.Currencies;
using OmniFi_API.Models.Cryptos;
using OmniFi_DTOs.Dtos.Banks;

namespace OmniFi_API.Controllers.Identity
{
    [Route($"api/{ControllerRouteNames.UserController}")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IRepository<FiatCurrency> _fiatCurrencyRepository;
        private readonly ILogger<UserController> _logger;

        public UserController(
            IUserRepository userRepository,
            ILogger<UserController> logger,
            IRepository<FiatCurrency> fiatCurrencyRepository)
        {
            _userRepository = userRepository;
            _logger = logger;
            _fiatCurrencyRepository = fiatCurrencyRepository;
        }

        [HttpPost(nameof(Login))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<LoginResponseDTO>>> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            var apiResponse = new ApiResponse<LoginResponseDTO>();

            try
            {

                var loginResponse = await _userRepository.Login(loginRequestDTO);

                if (loginResponse is null || loginResponse.User is null || string.IsNullOrEmpty(loginResponse.Token))
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    apiResponse.AddErrorMessage($"Invalid credentials\n\n" +
                        $"Your entered credentials may be incorrect or email sign-in is disabled for your account.\n\n" +
                        $"Please check your credentials and try again.");
                    return BadRequest(apiResponse);
                }

                apiResponse.StatusCode = HttpStatusCode.OK;
                apiResponse.IsSuccess = true;
                apiResponse.Result = loginResponse;
                return Ok(apiResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorMessages.ErrorPostMethodMessage
                    .Replace(ErrorMessages.VariableTag, nameof(Login)));
                apiResponse.IsSuccess = false;
                apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                apiResponse.AddErrorMessage(ErrorMessages.Error500Message);
                return StatusCode(500, apiResponse);
            }

        }

        [HttpPut(nameof(Put))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<ApiResponse<string?>>> Put(
            [FromBody] UserUpdateDTO userUpdateDTO
        )
        {

            var apiResponse = new ApiResponse<string?>();

            try
            {
                var user = await _userRepository.GetWithAllAccountsAsync(userUpdateDTO.UsernameOrEmail, tracked:true);

                if (user is null)
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.AddErrorMessage(ErrorMessages.ErrorUserNotFoundMessage
                        .Replace(ErrorMessages.VariableTag, userUpdateDTO.UsernameOrEmail));
                    apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(apiResponse);
                }

                if (userUpdateDTO.FiatCurrencyCode is not null)
                {

                    var fiatCurrency = await _fiatCurrencyRepository.GetAsync(
                        x => x.CurrencyCode == userUpdateDTO.FiatCurrencyCode);

                    if (fiatCurrency is null)
                    {
                        apiResponse.IsSuccess = false;
                        apiResponse.AddErrorMessage($"The fiat currency code '{userUpdateDTO.FiatCurrencyCode}' " +
                            $"is not available, please choose another currency");
                        apiResponse.StatusCode = HttpStatusCode.NotFound;
                        return NotFound(apiResponse);
                    }
                }

                if (user.Equals(userUpdateDTO))
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.AddErrorMessage("There aren't no properties to update");
                    apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(apiResponse);
                }

                await _userRepository.UpdateAsync(user, userUpdateDTO);
                
                return NoContent();


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorMessages.ErrorPostMethodMessage
                   .Replace(ErrorMessages.VariableTag, nameof(Put)));
                apiResponse.IsSuccess = false;
                apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                apiResponse.AddErrorMessage(ErrorMessages.Error500Message);
                return StatusCode(500, apiResponse);
            }
        }

        [HttpPost(nameof(Register))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<UserDTO>>> Register([FromBody] RegisterationRequestDTO registerationRequestDTO)
        {
            var apiResponse = new ApiResponse<UserDTO>();

            try
            {
                if (_userRepository.IsUserExistsByUserName(registerationRequestDTO.UserName))
                {
                    apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    apiResponse.IsSuccess = false;
                    apiResponse.AddErrorMessage($"username '{registerationRequestDTO.UserName}' already exists");
                    return BadRequest(apiResponse);
                }

                if (_userRepository.IsUserExistsByEmail(registerationRequestDTO.Email))
                {
                    apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    apiResponse.IsSuccess = false;
                    apiResponse.AddErrorMessage($"email '{registerationRequestDTO.Email}' already exists");
                    return BadRequest(apiResponse);
                }

                var response = await _userRepository.Register(registerationRequestDTO);

                if (!response.IsSucceeded)
                {

                    var errorMessages = ErrorMessages.ErrorPostMethodMessage.Replace(ErrorMessages.VariableTag, nameof(Register))  + " : "
                        + Environment.NewLine
                        +  string.Join(Environment.NewLine, response.ErrorMessages);

                    _logger.LogWarning(errorMessages);
                    apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                    apiResponse.IsSuccess = false;
                    apiResponse.AddErrorMessage(ErrorMessages.Error500Message);
                    return StatusCode(500, apiResponse);
                }

                apiResponse.IsSuccess = true;
                apiResponse.StatusCode = HttpStatusCode.Created;
                apiResponse.Result = response.User;
                return CreatedAtAction(nameof(Register), apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorMessages.ErrorPostMethodMessage
                    .Replace(ErrorMessages.VariableTag, nameof(Register)));
                apiResponse.IsSuccess = false;
                apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                apiResponse.AddErrorMessage(ErrorMessages.Error500Message);
                return StatusCode(500, apiResponse);
            }

        }

        [HttpDelete(nameof(Delete))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<ApiResponse<string?>>> Delete([Required] string usernameOrEmail)
        {

            var apiResponse = new ApiResponse<string?>();

            try
            {
                var user = await _userRepository.GetUserAsync(usernameOrEmail);

                if (user is null)
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.StatusCode = HttpStatusCode.NotFound;
                    apiResponse.AddErrorMessage(ErrorMessages.ErrorUserNotFoundMessage
                        .Replace(ErrorMessages.VariableTag, usernameOrEmail));
                    return NotFound(apiResponse);
                }

                await _userRepository.RemoveAsync(user);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorMessages.ErrorDeleteMethodMessage
                    .Replace(ErrorMessages.VariableTag, nameof(Delete)));
                apiResponse.IsSuccess = false;
                apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                apiResponse.AddErrorMessage(ErrorMessages.Error500Message);
                return StatusCode(500, apiResponse);
            }
        }

    }
}
