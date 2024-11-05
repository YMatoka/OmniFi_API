using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using OmniFi_DTOs.Dtos.Assets;
using OmniFi_API.Models.Assets;
using OmniFi_DTOs.Dtos.Api;
using OmniFi_API.Models.Cryptos;
using OmniFi_API.Models.Currencies;
using OmniFi_API.Repository;
using OmniFi_API.Repository.Interfaces;
using OmniFi_API.Utilities;
using System.Net;
using OmniFi_DTOs;
using System.ComponentModel.DataAnnotations;
using OmniFi_API.Controllers.Identity;

namespace OmniFi_API.Controllers.Assets
{
    [Route($"api/{ControllerRouteNames.FinancialAssetController}")]
    [ApiController]
    public class FinancialAssetController : ControllerBase
    {
        private readonly IFinancialAssetRepository _financialAssetRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRepository<AssetPlatform> _assetPlatform;
        private readonly IMapper _mapper;
        private readonly ILogger<FinancialAssetController> _logger;

        public FinancialAssetController(
            IFinancialAssetRepository financialAssetRepository,
            IMapper mapper,
            IUserRepository userRepository,
            IRepository<AssetPlatform> assetPlatform,
            ILogger<FinancialAssetController> logger)
        {
            _financialAssetRepository = financialAssetRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _assetPlatform = assetPlatform;
            _logger = logger;
        }

        [HttpGet(nameof(GetFinancialAssets))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<ApiResponse<IEnumerable<FinancialAssetDTO>>>> GetFinancialAssets([Required] string usernameOrEmail)
        {
            var apiResponse = new ApiResponse<IEnumerable<FinancialAssetDTO>>();

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

                var financialAssets = await _financialAssetRepository
                    .GetAllWithEntitiesAsync(
                    x => x.UserID == user.Id &&
                    x.IsAccountExists == true);

                var financialAssetDTOs = _mapper
                    .Map<IEnumerable<FinancialAssetDTO>>(financialAssets)
                    .OrderByDescending(x => x.Amount);

                apiResponse.IsSuccess = true;
                apiResponse.StatusCode = HttpStatusCode.OK;
                apiResponse.Result = financialAssetDTOs;

                return Ok(apiResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex , ErrorMessages.ErrorGetMethodMessage
                    .Replace(ErrorMessages.VariableTag, nameof(GetFinancialAssets)));
                apiResponse.IsSuccess = false;
                apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                apiResponse.AddErrorMessage(ErrorMessages.Error500Message);
                return StatusCode(500,apiResponse);
            }
        }

        [HttpGet(nameof(GetAggregatedFinancialAssets))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<ApiResponse<AggregatedFinancialAssetsDTO>>> GetAggregatedFinancialAssets([FromQuery] string usernameOrEmail)
        {
            var apiResponse = new ApiResponse<AggregatedFinancialAssetsDTO>();

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

                var financialAssets = await _financialAssetRepository
                    .GetAllWithEntitiesAsync(
                    x => x.UserID == user.Id && 
                    x.IsAccountExists == true);

                apiResponse.IsSuccess = true;
                apiResponse.StatusCode = HttpStatusCode.OK;
                apiResponse.Result = (new AggregatedFinancialAssetsDTO()
                {
                    FinancialAssetsAggregatedAmount = financialAssets.Sum(x => x.Amount)
                });

                return Ok(apiResponse);
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorMessages.ErrorGetMethodMessage
                    .Replace(ErrorMessages.VariableTag, nameof(GetAggregatedFinancialAssets)));
                apiResponse.IsSuccess = false;
                apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                apiResponse.AddErrorMessage(ErrorMessages.Error500Message);
                return StatusCode(500, apiResponse);
            }
        }

        [HttpGet(nameof(GetFinancialAssetsByAssetId))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<ApiResponse<FinancialAssetDTO>>> GetFinancialAssetsByAssetId(
            [Required] string usernameOrEmail,
            [Required] int financialAssetId)
        {

            var apiResponse = new ApiResponse<FinancialAssetDTO>();

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
                
                var financialAsset = await _financialAssetRepository
                    .GetWithEntitiesAsync(
                    x => x.UserID == user.Id && 
                    x.FinancialEntityId == financialAssetId && 
                    x.IsAccountExists == true);


                if (financialAsset is null)
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.AddErrorMessage($"The user '{user.UserName}' don't have a financial asset with the following id '{financialAssetId}'");
                    apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(apiResponse);
                }

                var financialAssetDTO = _mapper.Map<FinancialAssetDTO>(financialAsset);

                apiResponse.IsSuccess = true;
                apiResponse.StatusCode = HttpStatusCode.OK;
                apiResponse.Result = financialAssetDTO;

                return Ok(apiResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorMessages.ErrorGetMethodMessage
                    .Replace(ErrorMessages.VariableTag, nameof(GetFinancialAssetsByAssetId)));
                apiResponse.IsSuccess = false;
                apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                apiResponse.AddErrorMessage(ErrorMessages.Error500Message);
                return StatusCode(500, apiResponse);
            }
        }

    }
}
