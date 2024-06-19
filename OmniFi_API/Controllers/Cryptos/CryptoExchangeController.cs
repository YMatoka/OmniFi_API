using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using OmniFi_API.Data.Interfaces;
using OmniFi_API.Dtos.Cryptos;
using OmniFi_API.Models.Controllers;
using OmniFi_API.Models.Cryptos;
using OmniFi_API.Models.Encryption;
using OmniFi_API.Repository.Identity;
using OmniFi_API.Repository.Interfaces;
using OmniFi_API.Services.Interfaces;
using OmniFi_API.Utilities;
using System.Net;

namespace OmniFi_API.Controllers.Cryptos
{
    [Route($"api/{ControllerRouteNames.CryptoExchangeController}")]
    [ApiController]
    public class CryptoExchangeController : ControllerBase
    {
        private readonly IRepository<CryptoExchange> _cryptoExchangeRepository;
        private readonly ICryptoExchangeAccountRepository _cryptoExchangeAccountRepository;
        private readonly IUserRepository _userRepository;

        private ApiResponse _apiResponse;

        public CryptoExchangeController(
            IRepository<CryptoExchange> cryptoExchangeRepository,
            IUserRepository userRepository,
            ICryptoExchangeAccountRepository cryptoExchangeAccountRepository)
        {
            _cryptoExchangeRepository = cryptoExchangeRepository;
            _userRepository = userRepository;
            _cryptoExchangeAccountRepository = cryptoExchangeAccountRepository;

            _apiResponse = new();
        }

        [HttpPost(nameof(Create))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> Create([FromBody] CryptoExchangeAccountCreateDTO cryptoExchangeAccountCreateDTO)
        {
            var user = await _userRepository.GetUserAsync(cryptoExchangeAccountCreateDTO.UsernameOrEmail);

            if (user is null)
            {
                _apiResponse.IsSucess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add($"the username or email {cryptoExchangeAccountCreateDTO.UsernameOrEmail} is invalid");
                return BadRequest(_apiResponse);
            }

            var cryptoExchange = await _cryptoExchangeRepository.GetAsync(
                filter: (x) => x.ExchangeName.ToUpper() == cryptoExchangeAccountCreateDTO.CryptoExchangeName.ToUpper(),
                tracked:false);

            if (cryptoExchange is null)
            {
                _apiResponse.IsSucess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add($"the crypto exchange {cryptoExchangeAccountCreateDTO.CryptoExchangeName} does'nt exists in the database");
                return BadRequest(_apiResponse);
            }

            if (await _cryptoExchangeAccountRepository.GetAsync(
                (x) => x.UserID == user.Id 
                && x.CryptoExchangeID == cryptoExchange.CryptoExchangeID) is not null)
            {
                _apiResponse.IsSucess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add($"the user '{user.UserName}' already have a '{cryptoExchange.ExchangeName}' account");
                return BadRequest(_apiResponse);
            }

            var cryptoExchangeAccount = new CryptoExchangeAccount()
            {
                CryptoExchangeID = cryptoExchange.CryptoExchangeID,
                UserID = user.Id
            };

            await _cryptoExchangeAccountRepository.CreateAsync(cryptoExchangeAccount, cryptoExchangeAccountCreateDTO);

            //    (x) => x.CryptoApiCredentialId == credential.CryptoApiCredentialID);

            //var decryptedKey = await _stringEncryptionService.DecryptAsync( credential.ApiKey, aesKey!.Key);
            //var deryptedSecretdKey = await _stringEncryptionService.DecryptAsync(credential.ApiSecret, aesKey!.Key);

            _apiResponse.IsSucess = true;
            _apiResponse.StatusCode = HttpStatusCode.Created;
            return CreatedAtAction(nameof(Create), _apiResponse);

        }

    }
}
