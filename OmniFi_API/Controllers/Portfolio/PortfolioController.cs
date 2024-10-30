using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OmniFi_API.Models.Banks;
using OmniFi_API.Models.Cryptos;
using OmniFi_API.Repository.Interfaces;
using OmniFi_API.Services.Interfaces;
using OmniFi_API.Utilities;
using OmniFi_DTOs.Dtos.Api;
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
        ApiResponse _apiResponse;

        public PortfolioController(
            IFetchPortfolioService fetchPortfolioService,
            IUserRepository userRepository,
            ILogger<PortfolioController> logger)
        {
            _fetchPortfolioService = fetchPortfolioService;

            _userRepository = userRepository;

            _apiResponse = new();
            _logger = logger;
        }

        [HttpPost(nameof(FetchPortfolio))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<ApiResponse>> FetchPortfolio([Required] string usernameOrEmail)
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

                await _fetchPortfolioService.FetchPortfolio(usernameOrEmail);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorMessages.ErrorPostMethodMessage
                    .Replace(ErrorMessages.VariableTag, nameof(FetchPortfolio)));
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.AddErrorMessage(ErrorMessages.Error500Message);
                return StatusCode(500, _apiResponse);
            }

        }
    }
}
