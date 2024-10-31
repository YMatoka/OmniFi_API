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
using OmniFi_API.Models.Banks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using OmniFi_DTOs.Dtos.Identity;

namespace OmniFi_API.Controllers.Cryptos
{
    [Route($"api/{ControllerRouteNames.CryptoExchangeAccountController}")]
    [ApiController]
    public class CryptoExchangeAccountController : ControllerBase
    {
        private readonly IRepository<CryptoExchange> _cryptoExchangeRepository;
        private readonly ICryptoExchangeAccountRepository _cryptoExchangeAccountRepository;
        private readonly IFinancialAssetRepository _financialAssetRepository;
        private readonly IUserRepository _userRepository;
        private readonly IStringEncryptionService _stringEncryptionService;
        private readonly ILogger<CryptoExchangeAccountController> _logger;

        private ApiResponse _apiResponse;

        public CryptoExchangeAccountController(
            IRepository<CryptoExchange> cryptoExchangeRepository,
            IUserRepository userRepository,
            ICryptoExchangeAccountRepository cryptoExchangeAccountRepository,
            IFinancialAssetRepository financialAssetRepository,
            ILogger<CryptoExchangeAccountController> logger,
            IStringEncryptionService stringEncryptionService)
        {
            _cryptoExchangeRepository = cryptoExchangeRepository;
            _userRepository = userRepository;
            _cryptoExchangeAccountRepository = cryptoExchangeAccountRepository;
            _financialAssetRepository = financialAssetRepository;
            _apiResponse = new();
            _logger = logger;
            _stringEncryptionService = stringEncryptionService;
        }

        [HttpPost(nameof(Create))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<ApiResponse>> Create([FromBody] CryptoExchangeAccountCreateDTO  cryptoExchangeAccountCreateDTO)
        {
            try
            {
                var user = await _userRepository.GetUserAsync(cryptoExchangeAccountCreateDTO.UsernameOrEmail);

                if (user is null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.AddErrorMessage(ErrorMessages.ErrorUserNotFoundMessage
                        .Replace(ErrorMessages.VariableTag, cryptoExchangeAccountCreateDTO.UsernameOrEmail));
                    return NotFound(_apiResponse);
                }

                var cryptoExchange = await _cryptoExchangeRepository.GetAsync(
                    filter: (x) => x.ExchangeName.ToUpper() == cryptoExchangeAccountCreateDTO.CryptoExchangeName.ToUpper(),
                    tracked: false);

                if (cryptoExchange is null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.AddErrorMessage($"the crypto exchange '{cryptoExchangeAccountCreateDTO.CryptoExchangeName}' does'nt exists in the database");
                    return NotFound(_apiResponse);
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

                var financialAssets = await _financialAssetRepository.GetAllWithEntitiesAsync(
                        x => x.UserID == x.UserID &&
                        x.AssetPlatform!.CryptoExchange!.ExchangeName == cryptoExchange.ExchangeName);

                foreach (var financialAsset in financialAssets)
                {
                    await _financialAssetRepository.UpdateAsync(financialAsset: financialAsset, isAccountExists: true);
                }

                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode = HttpStatusCode.Created;
                return CreatedAtAction(nameof(Create), _apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorMessages.ErrorCreateMethodMessage
                    .Replace(ErrorMessages.VariableTag, nameof(Create)));
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.AddErrorMessage(ErrorMessages.Error500Message);
                return StatusCode(500, _apiResponse);
            }
        }

        [HttpPut(nameof(Put))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<ApiResponse>> Put(
            [FromBody] CryptoExchangeAccountUpdateDTO cryptoExchangeAccountUpdateDTO
            )
        {
            try
            {
                var user = await _userRepository.GetUserAsync(cryptoExchangeAccountUpdateDTO.UsernameOrEmail);

                if (user is null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.AddErrorMessage(ErrorMessages.ErrorUserNotFoundMessage
                        .Replace(ErrorMessages.VariableTag, cryptoExchangeAccountUpdateDTO.UsernameOrEmail));
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_apiResponse);
                }

                var cryptoExchange = await _cryptoExchangeRepository.GetAsync(
                    filter: (x) => x.ExchangeName.ToUpper() == cryptoExchangeAccountUpdateDTO.CryptoExchangeName.ToUpper(),
                    tracked: false);

                if (cryptoExchange is null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.AddErrorMessage($"the crypto exchange '{cryptoExchangeAccountUpdateDTO.CryptoExchangeName}' does'nt exists in the database");
                    return NotFound(_apiResponse);
                }

                var cryptoExchangeAccount = await _cryptoExchangeAccountRepository.GetWithEntitiesAsync(
                    (x) => x.UserID == user.Id
                    && x.CryptoExchangeID == cryptoExchange.CryptoExchangeID);


                if (cryptoExchangeAccount is null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.AddErrorMessage($"the user '{user.UserName}' doesn't have a '{cryptoExchange.ExchangeName}' account");
                    return NotFound(_apiResponse);
                }

                var isCryptoExchangeEqual = await IsCryptoExchangeEqual(
                    cryptoExchangeAccount,
                    cryptoExchangeAccountUpdateDTO);

                if(isCryptoExchangeEqual is not null && 
                    isCryptoExchangeEqual == true)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.AddErrorMessage("There aren't no properties to update");
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }

                if (isCryptoExchangeEqual is null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.AddErrorMessage(ErrorMessages.Error500Message);
                    _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(500,_apiResponse);
                }

                _cryptoExchangeAccountRepository;

                return NoContent();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorMessages.ErrorPostMethodMessage
                   .Replace(ErrorMessages.VariableTag, nameof(Register)));
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.AddErrorMessage(ErrorMessages.Error500Message);
                return StatusCode(500, _apiResponse);
            }
        }

        private async Task<bool?> IsCryptoExchangeEqual(
            CryptoExchangeAccount cryptoExchangeAccount,
            CryptoExchangeAccountUpdateDTO cryptoExchangeAccountUpdateDTO)
        {
            var aesKey = cryptoExchangeAccount.CryptoApiCredential?.AesKey?.Key;
            var aesIV = cryptoExchangeAccount.CryptoApiCredential?.AesIV?.IV;

            if (aesKey == null ||
                aesIV == null ||
                cryptoExchangeAccount.CryptoApiCredential?.ApiKey == null ||
                cryptoExchangeAccount?.CryptoApiCredential?.ApiSecret == null)
                return null;

            var actualApiKey = await _stringEncryptionService.DecryptAsync(
                cryptoExchangeAccount.CryptoApiCredential.ApiKey, aesKey, aesIV!);

            var actualApiSecret = await _stringEncryptionService.DecryptAsync(
                cryptoExchangeAccount.CryptoApiCredential.ApiSecret, aesKey, aesIV!);

            return actualApiKey != cryptoExchangeAccountUpdateDTO.ApiKey || actualApiSecret != cryptoExchangeAccountUpdateDTO.ApiSecret ?
                true :
                false;
        }

        [HttpDelete(nameof(Delete))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.AddErrorMessage(ErrorMessages.ErrorUserNotFoundMessage
                        .Replace(ErrorMessages.VariableTag, usernameOrMail));
                    return NotFound(_apiResponse);
                }

                var cryptoExchange = await _cryptoExchangeRepository.GetAsync(
                    filter: (x) => x.ExchangeName.ToUpper() == cryptoExchangeName.ToUpper(),
                    tracked: false);

                if (cryptoExchange is null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.AddErrorMessage($"the crypto exchange '{cryptoExchangeName}' does'nt exists in the database");
                    return NotFound(_apiResponse);
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

                var financialAssets = await _financialAssetRepository.GetAllWithEntitiesAsync(
                    x => x.UserID == x.UserID &&
                    x.AssetPlatform!.CryptoExchange!.ExchangeName == cryptoExchangeName);

                foreach(var financialAsset in financialAssets)
                {
                    await _financialAssetRepository.UpdateAsync(financialAsset: financialAsset, isAccountExists:false);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorMessages.ErrorDeleteMethodMessage
                    .Replace(ErrorMessages.VariableTag, nameof(Delete)));
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.AddErrorMessage(ErrorMessages.Error500Message);
                return StatusCode(500, _apiResponse);
            }
        }

    }
}
