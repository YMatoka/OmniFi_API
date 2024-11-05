using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OmniFi_DTOs.Dtos.Cryptos;
using OmniFi_DTOs.Dtos.Api;
using OmniFi_API.Models.Cryptos;
using OmniFi_API.Repository.Interfaces;
using OmniFi_API.Utilities;
using System.Net;
using System.ComponentModel.DataAnnotations;

namespace OmniFi_API.Controllers.Cryptos
{
    [Route($"api/{ControllerRouteNames.CryptoExchangeController}")]
    [ApiController]
    public class CryptoExchangeController : ControllerBase
    {
        private readonly IRepository<CryptoExchange> _cryptoRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CryptoExchangeController> _logger;
        private ApiResponse apiResponse;


        public CryptoExchangeController(IRepository<CryptoExchange> cryptoRepository, IMapper mapper, ILogger<CryptoExchangeController> logger)
        {
            _cryptoRepository = cryptoRepository;
            _mapper = mapper;
            apiResponse = new ApiResponse();
            _logger = logger;
        }

        [HttpGet(nameof(GetCryptoExchange))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> GetCryptoExchange([Required] int id)
        {
            try
            {
                if (id == 0)
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    apiResponse.AddErrorMessage("The id 0 is not valid");
                    return BadRequest(apiResponse);
                }

                var cryptoExchange = await _cryptoRepository.GetAsync(x => x.CryptoExchangeID == id);

                if (cryptoExchange is null)
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.StatusCode = HttpStatusCode.NotFound;
                    apiResponse.AddErrorMessage($"There isn't a crypto exchange with the id {id}");
                    return NotFound(apiResponse);
                }

                apiResponse.IsSuccess = true;
                apiResponse.StatusCode = HttpStatusCode.OK;
                apiResponse.Result = _mapper.Map<CryptoExchangeDTO>(cryptoExchange);

                return Ok(apiResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorMessages.ErrorGetMethodMessage
                    .Replace(ErrorMessages.VariableTag, nameof(GetCryptoExchange)));
                apiResponse.IsSuccess = false;
                apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                apiResponse.AddErrorMessage(ErrorMessages.Error500Message);
                return StatusCode(500, apiResponse);
            }
        }

        [HttpGet(nameof(GetCryptoExchanges))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> GetCryptoExchanges()
        {

            try
            {
                var cryptoExchanges = await _cryptoRepository.GetAllAsync();

                apiResponse.IsSuccess = true;
                apiResponse.StatusCode= HttpStatusCode.OK;
                apiResponse.Result = _mapper.Map<IEnumerable<CryptoExchangeDTO>>(cryptoExchanges);

                return Ok(apiResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorMessages.ErrorGetMethodMessage
                    .Replace(ErrorMessages.VariableTag, nameof(GetCryptoExchanges)));
                apiResponse.IsSuccess = false;
                apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                apiResponse.AddErrorMessage(ErrorMessages.Error500Message);
                return StatusCode(500, apiResponse);
            }

        }
    }



}

