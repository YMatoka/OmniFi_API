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
        private ApiResponse _apiResponse;

        public FinancialAssetController(
            IFinancialAssetRepository financialAssetRepository,
            IMapper mapper,
            IUserRepository userRepository,
            IRepository<AssetPlatform> assetPlatform)
        {
            _financialAssetRepository = financialAssetRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _apiResponse = new ApiResponse();
            _assetPlatform = assetPlatform;
        }

        [HttpGet(nameof(GetFinancialAssets))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<ApiResponse>> GetFinancialAssets([Required] string usernameOrEmail)
        {
            try
            {
                var user = await _userRepository.GetUserAsync(usernameOrEmail);

                if (user is null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.AddErrorMessage($"the usernameOrEmail or email '{usernameOrEmail}' does not exists");
                    return NotFound(_apiResponse);
                }

                var financialAssets = await _financialAssetRepository
                    .GetAllWithEntitiesAsync((x) => x.UserID == user.Id);

                var financialAssetDTOs = _mapper
                    .Map<IEnumerable<FinancialAssetDTO>>(financialAssets)
                    .OrderByDescending(x => x.Amount);

                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                _apiResponse.Result = financialAssetDTOs;

                return Ok(_apiResponse);

            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.AddErrorMessage(ex.Message);
                return _apiResponse;
            }
        }

        [HttpGet(nameof(GetAggregatedFinancialAssets))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<ApiResponse>> GetAggregatedFinancialAssets([FromQuery] string usernameOrEmail)
        {
            try
            {
                var user = await _userRepository.GetUserAsync(usernameOrEmail);

                if (user is null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.AddErrorMessage($"the usernameOrEmail or email '{usernameOrEmail}' does not exists");
                    return NotFound(_apiResponse);
                }

                var financialAssets = await _financialAssetRepository
                    .GetAllWithEntitiesAsync((x) => x.UserID == user.Id);

                _apiResponse.Result = (new AggregatedFinancialAssetsDTO()
                {
                    FinancialAssetsAggregatedAmount = financialAssets.Sum(x => x.Amount)
                });

                return Ok(_apiResponse);
                
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.AddErrorMessage(ex.Message);
                return _apiResponse;
            }
        }

        [HttpGet(nameof(GetFinancialAssetsByAssetId))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<ApiResponse>> GetFinancialAssetsByAssetId(
            [Required] string usernameOrEmail,
            [Required] int financialAssetId)
        {
            try
            {
                var user = await _userRepository.GetUserAsync(usernameOrEmail);

                if (user is null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.AddErrorMessage($"the username or email '{usernameOrEmail}' does not exists");
                    return NotFound(_apiResponse);
                }

                var financialAsset = await _financialAssetRepository
                    .GetWithEntitiesAsync((x) => x.UserID == user.Id && x.FinancialEntityId == financialAssetId);


                if (financialAsset is null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.AddErrorMessage($"the user '{user.UserName}' don't have a financial asset with the following id '{financialAssetId}'");
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_apiResponse);
                }

                var financialAssetDTO = _mapper.Map<FinancialAssetDTO>(financialAsset);

                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                _apiResponse.Result = financialAssetDTO;

                return Ok(_apiResponse);

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
