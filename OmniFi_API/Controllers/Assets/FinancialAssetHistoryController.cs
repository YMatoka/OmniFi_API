﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OmniFi_DTOs.Dtos.Assets;
using OmniFi_DTOs.Dtos.Api;
using OmniFi_API.Repository.Interfaces;
using OmniFi_API.Utilities;
using System.Net;
using System.Runtime.InteropServices;
using System.ComponentModel.DataAnnotations;

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
        private readonly ILogger<FinancialAssetHistoryController> _logger;
        private ApiResponse _apiResponse;

        public FinancialAssetHistoryController(
            IFinancialAssetHistoryRepository financialAssetHistoryRepository,
            IUserRepository userRepository,
            IMapper mapper,
            IFinancialAssetRepository financialAssetRepository,
            ILogger<FinancialAssetHistoryController> logger)
        {
            _financialAssetHistoryRepository = financialAssetHistoryRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _apiResponse = new ApiResponse();
            _financialAssetRepository = financialAssetRepository;
            _logger = logger;
        }


        [HttpGet(nameof(GetAllFinancialHistoryAsset))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<ApiResponse>> GetAllFinancialHistoryAsset([Required] string usernameOrEmail)
        {
            try
            {
                var user = await _userRepository.GetUserAsync(usernameOrEmail);

                if (user is null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.AddErrorMessage(ErrorMessages.ErrorUserNotFoundMessage
                        .Replace(ErrorMessages.VariableTag, usernameOrEmail));
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
                _logger.LogError(ex, ErrorMessages.ErrorGetMethodMessage
                    .Replace(ErrorMessages.VariableTag, nameof(GetAllFinancialHistoryAsset)));
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.AddErrorMessage(ErrorMessages.Error500Message);
                return StatusCode(500,_apiResponse);
            }
        }

        [HttpGet(nameof(GetFinancialHistoryByAssetId))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<ApiResponse>> GetFinancialHistoryByAssetId(
            [Required] string usernameOrEmail, 
            [Required] int financialAssetId)
        {
            try
            {
                var user = await _userRepository.GetUserAsync(usernameOrEmail);

                if (user is null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.AddErrorMessage(ErrorMessages.ErrorUserNotFoundMessage
                        .Replace(ErrorMessages.VariableTag, usernameOrEmail));
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_apiResponse);
                }

                var financialAsset = await _financialAssetRepository
                    .GetAllWithEntitiesAsync((x) => x.FinancialEntityId == financialAssetId && x.UserID == user.Id);

                if (financialAsset is null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.AddErrorMessage($"the user '{usernameOrEmail}' don't have a financial asset with the following id '{financialAssetId}'");
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
                _logger.LogError(ex, ErrorMessages.ErrorGetMethodMessage
                    .Replace(ErrorMessages.VariableTag, nameof(GetFinancialHistoryByAssetId)));
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.AddErrorMessage(ErrorMessages.Error500Message);
                return StatusCode(500, _apiResponse);
            }

        }

    }
}
