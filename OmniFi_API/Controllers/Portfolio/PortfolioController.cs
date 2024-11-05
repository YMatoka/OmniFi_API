using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OmniFi_API.Models.Banks;
using OmniFi_API.Models.Cryptos;
using OmniFi_API.Repository.Interfaces;
using OmniFi_API.Services.Interfaces;
using OmniFi_API.Utilities;
using OmniFi_DTOs.Dtos.Api;
using OmniFi_DTOs.Dtos.Banks;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace OmniFi_API.Controllers.Portfolio
{
    [Route($"api/{ControllerRouteNames.PortfolioController}")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        IFetchPortfolioService _fetchPortfolioService;

        IUserRepository _userRepository;
        private readonly ILogger<PortfolioController> _logger;

        public PortfolioController(
            IFetchPortfolioService fetchPortfolioService,
            IUserRepository userRepository,
            ILogger<PortfolioController> logger)
        {
            _fetchPortfolioService = fetchPortfolioService;

            _userRepository = userRepository;

            _logger = logger;
        }

        [HttpPost(nameof(FetchPortfolio))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<ApiResponse<string?>>> FetchPortfolio([Required] string usernameOrEmail)
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

                await _fetchPortfolioService.FetchPortfolio(usernameOrEmail);

                return NoContent();
            }
            catch(UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, ErrorMessages.ErrorPostMethodMessage
                    .Replace(ErrorMessages.VariableTag, nameof(FetchPortfolio)));
                apiResponse.IsSuccess = false;
                apiResponse.StatusCode = HttpStatusCode.Unauthorized;
                apiResponse.AddErrorMessage(ex.Message);
                return Unauthorized(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorMessages.ErrorPostMethodMessage
                    .Replace(ErrorMessages.VariableTag, nameof(FetchPortfolio)));
                apiResponse.IsSuccess = false;
                apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                apiResponse.AddErrorMessage(ErrorMessages.Error500Message);
                return StatusCode(500, apiResponse);
            }

        }
    }
}
