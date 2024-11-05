
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OmniFi_DTOs.Dtos.Banks;
using OmniFi_API.Models.Banks;
using OmniFi_DTOs.Dtos.Api;
using OmniFi_API.Repository.Interfaces;
using OmniFi_API.Utilities;
using System.Net;
using System.ComponentModel.DataAnnotations;

namespace OmniFi_API.Controllers.Banks
{
    [Route($"api/{ControllerRouteNames.BankController}")]
    [ApiController]
    public class BankController : ControllerBase
    {
        private readonly IRepository<Bank> _bankRepository;
        private ApiResponse apiResponse;
        private readonly IMapper _mapper;
        private readonly ILogger<BankController> _logger;

        public BankController(IRepository<Bank> bankRepository, IMapper mapper, ILogger<BankController> logger)
        {
            _bankRepository = bankRepository;
            apiResponse = new ApiResponse();
            _mapper = mapper;
            _logger = logger;
        }


        [HttpGet(nameof(GetBank))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> GetBank([Required] int id)
        {
            try
            {
                if(id == 0)
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    apiResponse.AddErrorMessage("The id 0 is not valid");
                    return BadRequest(apiResponse);
                }

                var bank = await _bankRepository.GetAsync(x => x.BankID == id);

                if(bank is null)
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.StatusCode =HttpStatusCode.NotFound;
                    apiResponse.AddErrorMessage($"There isn't a bank with the id {id}");
                    return NotFound(apiResponse);
                }

                apiResponse.IsSuccess = true;
                apiResponse.StatusCode=HttpStatusCode.OK;
                apiResponse.Result = _mapper.Map<BankDTO>(bank);

                return Ok(apiResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorMessages.ErrorGetMethodMessage
                    .Replace(ErrorMessages.VariableTag, nameof(GetBank)));
                apiResponse.IsSuccess = false;
                apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                apiResponse.AddErrorMessage(ErrorMessages.Error500Message);
                return StatusCode(500, apiResponse);
            }
        }

        [HttpGet(nameof(GetBanks))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> GetBanks()
        {
            try
            {
                var banks = await _bankRepository.GetAllAsync();

                apiResponse.IsSuccess = true;
                apiResponse.StatusCode = HttpStatusCode.OK;
                apiResponse.Result = _mapper.Map<IEnumerable<BankDTO>>(banks);

                return Ok(apiResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorMessages.ErrorGetMethodMessage
                    .Replace(ErrorMessages.VariableTag, nameof(GetBanks)));
                apiResponse.IsSuccess = false;
                apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                apiResponse.AddErrorMessage(ErrorMessages.Error500Message);
                return StatusCode(500, apiResponse);
            }
        }

    }
}
