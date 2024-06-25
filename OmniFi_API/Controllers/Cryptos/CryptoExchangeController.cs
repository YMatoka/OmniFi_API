using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OmniFi_DTOs.Dtos.Cryptos;
using OmniFi_API.Models.Controllers;
using OmniFi_API.Models.Cryptos;
using OmniFi_API.Repository.Interfaces;
using OmniFi_API.Utilities;
using System.Net;

namespace OmniFi_API.Controllers.Cryptos
{
    [Route($"api/{ControllerRouteNames.CryptoExchangeController}")]
    [ApiController]
    public class CryptoExchangeController : ControllerBase
    {
        private readonly IRepository<CryptoExchange> _cryptoRepository;
        private readonly IMapper _mapper;
        private ApiResponse _apiResponse;

        public CryptoExchangeController(IRepository<CryptoExchange> cryptoRepository, IMapper mapper)
        {
            _cryptoRepository = cryptoRepository;
            _mapper = mapper;
            _apiResponse = new ApiResponse();
        }

        [HttpGet("{id:int}", Name = nameof(GetCryptoExchange))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse>> GetCryptoExchange(int id)
        {
            try
            {
                if (id == 0)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }

                var cryptoExchange = await _cryptoRepository.GetAsync(x => x.CryptoExchangeID == id);

                if (cryptoExchange is null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_apiResponse);
                }

                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                _apiResponse.Result = _mapper.Map<CryptoExchangeDTO>(cryptoExchange);

                return Ok(_apiResponse);

            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.AddErrorMessage(ex.Message);
                return _apiResponse;
            }
        }

        [HttpGet(nameof(GetCryptoExchanges))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse>> GetCryptoExchanges()
        {

            try
            {
                var cryptoExchanges = await _cryptoRepository.GetAllAsync();

                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode= HttpStatusCode.OK;
                _apiResponse.Result = _mapper.Map<IEnumerable<CryptoExchangeDTO>>(cryptoExchanges);

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

