using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using OmniFi_API.Data.Interfaces;
using OmniFi_DTOs.Dtos.Cryptos;
using OmniFi_DTOs.Dtos.Api;
using OmniFi_API.Models.Cryptos;
using OmniFi_API.Models.Encryption;
using OmniFi_API.Repository.Identity;
using OmniFi_API.Repository.Interfaces;
using OmniFi_API.Services.Interfaces;
using OmniFi_API.Utilities;
using System.Net;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;

namespace OmniFi_API.Controllers.Cryptos
{
    [Route($"api/{ControllerRouteNames.CryptoExchangeAccountController}")]
    [ApiController]
    public class CryptoExchangeAccountController : ControllerBase
    {
        private readonly IRepository<CryptoExchange> _cryptoExchangeRepository;
        private readonly ICryptoExchangeAccountRepository _cryptoExchangeAccountRepository;
        private readonly IUserRepository _userRepository;

        private ApiResponse _apiResponse;

        public CryptoExchangeAccountController(
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
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<ApiResponse>> Create([FromBody] CryptoExchangeAccountCreateDTO  cryptoExchangeAccountCreateDTO)
        {
            try
            {
                var user = await _userRepository.GetUserAsync(cryptoExchangeAccountCreateDTO.UsernameOrEmail);

                if (user is null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.AddErrorMessage($"the username or email '{cryptoExchangeAccountCreateDTO.UsernameOrEmail}' is invalid");
                    return BadRequest(_apiResponse);
                }

                var cryptoExchange = await _cryptoExchangeRepository.GetAsync(
                    filter: (x) => x.ExchangeName.ToUpper() == cryptoExchangeAccountCreateDTO.CryptoExchangeName.ToUpper(),
                    tracked: false);

                if (cryptoExchange is null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.AddErrorMessage($"the crypto exchange '{cryptoExchangeAccountCreateDTO.CryptoExchangeName}' does'nt exists in the database");
                    return BadRequest(_apiResponse);
                }

                if (await _cryptoExchangeAccountRepository.GetAsync(
                    (x) => x.UserID == user.Id
                    && x.CryptoExchangeID == cryptoExchange.CryptoExchangeID) is not null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.AddErrorMessage($"the user '{user.UserName}' already have a '{cryptoExchange.ExchangeName}' account");
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

                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode = HttpStatusCode.Created;
                return CreatedAtAction(nameof(Create), _apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.AddErrorMessage(ex.Message);
                return _apiResponse;
            }



        }

        [HttpDelete(nameof(Delete))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles= Roles.User)]
        public async Task<ActionResult<ApiResponse>> Delete(
            [Required] string usernameOrMail,
            [Required] string cryptoExchangeName)
        {
            try
            {
                var user = await _userRepository.GetUserAsync(usernameOrMail);

                if (user is null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.AddErrorMessage($"the username or email '{usernameOrMail}' is invalid");
                    return BadRequest(_apiResponse);
                }

                var cryptoExchange = await _cryptoExchangeRepository.GetAsync(
                    filter: (x) => x.ExchangeName.ToUpper() == cryptoExchangeName.ToUpper(),
                    tracked: false);

                if (cryptoExchange is null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.AddErrorMessage($"the crypto exchange '{cryptoExchangeName}' does'nt exists in the database");
                    return BadRequest(_apiResponse);
                }


                var cryptoExchangeAccount = await _cryptoExchangeAccountRepository
                    .GetWithEntitiesAsync(filter: x => x.CryptoExchange!.ExchangeName == cryptoExchangeName &&
                    x.User!.Id == user.Id, tracked:true);

                if (cryptoExchangeAccount is null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.AddErrorMessage($"the user '{user.UserName}' doesn't have a '{cryptoExchangeName}' account in the database");
                    return BadRequest(_apiResponse);
                }

                await _cryptoExchangeAccountRepository.RemoveAsync(cryptoExchangeAccount);

                _apiResponse.StatusCode = HttpStatusCode.OK;

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
