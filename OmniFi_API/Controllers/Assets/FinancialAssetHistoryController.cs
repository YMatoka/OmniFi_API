using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;
using OmniFi_API.Dtos.Assets;
using OmniFi_API.Models.Controllers;
using OmniFi_API.Repository.Interfaces;
using OmniFi_API.Utilities;
using System.Net;
using System.Runtime.InteropServices;

namespace OmniFi_API.Controllers.Assets
{
    [Route($"api/{ControllerRouteNames.FinancialAssetHistoryController}")]
    [ApiController]
    public class FinancialAssetHistoryController : ControllerBase
    {
        private readonly IFinancialAssetHistoryRepository _financialAssetHistoryRepository;
        private readonly IFinancialAssetRepository _financialAssetRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private ApiResponse _apiResponse;
        public FinancialAssetHistoryController(
            IFinancialAssetHistoryRepository financialAssetHistoryRepository,
            IUserRepository userRepository,
            IMapper mapper,
            IFinancialAssetRepository financialAssetRepository)
        {
            _financialAssetHistoryRepository = financialAssetHistoryRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _apiResponse = new ApiResponse();
            _financialAssetRepository = financialAssetRepository;
        }


        [HttpGet("{username}", Name = nameof(GetAllFinancialHistoryAsset))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<ApiResponse>> GetAllFinancialHistoryAsset(string username)
        {
            try
            {
                var user = await _userRepository.GetUserAsync(username);

                if (user is null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.AddErrorMessage($"the username '{username}' does not exists");
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_apiResponse);
                }

                var financialHistoryAssets = await _financialAssetHistoryRepository
                    .GetAllWithEntitiesAsync(x => x.UserID == user.Id);


                var financialHistoryAssetsDTO = _mapper.Map<IEnumerable<FinancialAssetHistoryDTO>>(financialHistoryAssets)
                    .OrderBy(x => x.RecordedAt)
                    .GroupBy(x => x.FinancialAssetId);

                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                _apiResponse.Result = financialHistoryAssetsDTO;

                return Ok(_apiResponse);

            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.AddErrorMessage(ex.Message);
                return BadRequest(_apiResponse);
            }
        }

        [HttpGet("{username}/{financialAssetId:int}",  Name = nameof(GetFinancialHistoryByAssetId))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<ApiResponse>> GetFinancialHistoryByAssetId(string username, int financialAssetId)
        {
            try
            {
                var user = await _userRepository.GetUserAsync(username);

                if (user is null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.AddErrorMessage($"the username '{username}' does not exists");
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_apiResponse);
                }

                var financialAsset = await _financialAssetRepository
                    .GetAllWithEntitiesAsync((x) => x.FinancialEntityId == financialAssetId && x.UserID == user.Id);

                if (financialAsset is null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.AddErrorMessage($"the user '{username}' don't have a financial asset with the following Id '{financialAssetId}'");
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_apiResponse);
                }

                var financialAssetsHistory = await _financialAssetHistoryRepository
                    .GetAllWithEntitiesAsync((x) => x.UserID == user.Id && x.FinancialAssetId == financialAssetId);


                var financialAssetsHistoryDTO = _mapper.Map<IEnumerable<FinancialAssetHistoryDTO>>(financialAssetsHistory)
                    .OrderBy(x => x.RecordedAt);

                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                _apiResponse.Result = financialAssetsHistoryDTO;

                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.AddErrorMessage(ex.Message);
                return BadRequest(_apiResponse);
            }

        }

    }
}
