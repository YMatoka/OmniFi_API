using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OmniFi_API.Models.Banks;
using OmniFi_API.Models.Cryptos;
using OmniFi_API.Repository.Interfaces;
using OmniFi_API.Services.Interfaces;
using OmniFi_API.Utilities;
using OmniFi_DTOs.Dtos.Api;
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

        [HttpPost("{username}",  Name = nameof(FetchPortfolio))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<ApiResponse>> FetchPortfolio(string username)
        {
            var user = await _userRepository.GetUserAsync(username);

            if (user is null)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.AddErrorMessage($"the user '{username}' does'nt exists");
                return BadRequest(_apiResponse);
            }

            await _fetchPortfolioService.FetchPortfolio(username);

            _apiResponse.IsSuccess = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;

            return Ok(_apiResponse);
        }
    }
}
