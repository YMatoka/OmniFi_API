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

        ApiResponse _apiResponse;
        public PortfolioController(
            IFetchPortfolioService fetchPortfolioService,
            IUserRepository userRepository)
        {
            _fetchPortfolioService = fetchPortfolioService;

            _userRepository = userRepository;

            _apiResponse = new();
        }

        [HttpPost(nameof(FetchPortfolio))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<ApiResponse>> FetchPortfolio([Required] string usernameOrEmail)
        {
            var user = await _userRepository.GetUserAsync(usernameOrEmail);

            if (user is null)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.AddErrorMessage($"the user or email '{usernameOrEmail}' does'nt exists");
                return BadRequest(_apiResponse);
            }

            await _fetchPortfolioService.FetchPortfolio(usernameOrEmail);

            _apiResponse.IsSuccess = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;

            return Ok(_apiResponse);
        }
    }
}
