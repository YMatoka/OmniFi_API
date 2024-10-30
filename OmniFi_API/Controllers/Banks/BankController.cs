
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
        private ApiResponse _apiResponse;
        private readonly IMapper _mapper;
        private readonly ILogger<BankController> _logger;

        public BankController(IRepository<Bank> bankRepository, IMapper mapper, ILogger<BankController> logger)
        {
            _bankRepository = bankRepository;
            _apiResponse = new ApiResponse();
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
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.AddErrorMessage("The id 0 is not valid");
                    return BadRequest(_apiResponse);
                }

                var bank = await _bankRepository.GetAsync(x => x.BankID == id);

                if(bank is null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode =HttpStatusCode.NotFound;
                    _apiResponse.AddErrorMessage($"There isn't a bank with the id {id}");
                    return NotFound(_apiResponse);
                }

                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode=HttpStatusCode.OK;
                _apiResponse.Result = _mapper.Map<BankDTO>(bank);

                return Ok(_apiResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorMessages.ErrorGetMethodMessage
                    .Replace(ErrorMessages.VariableTag, nameof(GetBank)));
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.AddErrorMessage(ErrorMessages.Error500Message);
                return StatusCode(500, _apiResponse);
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

                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                _apiResponse.Result = _mapper.Map<IEnumerable<BankDTO>>(banks);

                return Ok(_apiResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorMessages.ErrorGetMethodMessage
                    .Replace(ErrorMessages.VariableTag, nameof(GetBanks)));
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.AddErrorMessage(ErrorMessages.Error500Message);
                return StatusCode(500, _apiResponse);
            }
        }

    }
}
