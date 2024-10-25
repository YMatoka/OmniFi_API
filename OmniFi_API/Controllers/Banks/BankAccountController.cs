using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OmniFi_DTOs.Dtos.Banks;
using OmniFi_API.Models.Banks;
using OmniFi_DTOs.Dtos.Api;
using OmniFi_API.Repository.Interfaces;
using OmniFi_API.Utilities;
using System.Net;
using OmniFi_API.Services.Interfaces;
using OmniFi_API.Services.Api.Banks;
using OmniFi_API.Options.Banks;
using Microsoft.Extensions.Options;
using OmniFi_API.DTOs.GoCardless;
using System.ComponentModel.DataAnnotations;
using static System.Net.WebRequestMethods;
using OmniFi_API.Repository.Banks;
using OmniFi_API.Repository.Cryptos;
using Microsoft.AspNetCore.Mvc.TagHelpers;

namespace OmniFi_API.Controllers.Banks
{
    [Route($"api/{ControllerRouteNames.BankAccountController}")]
    [ApiController]
    public class BankAccountController : ControllerBase
    {
        private readonly IRepository<Bank> _banksRepository;
        private readonly IUserRepository _userRepository;
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly IBankInfoService _bankInfoService;
        private readonly IFinancialAssetRepository _financialAssetRepository;

        private readonly IRepository<BankSubAccount> _bankSubAccountRepository;

        private readonly IEqualityComparer<BankSubAccount> _bankSubAccountEqualityComparer;

        private readonly BankInfoServiceOptions _bankInfoServiceOptions;
        private readonly GocardlessBankInfoOptions _goCardlessOptions;


        private const string BankInfoTag = "{BankName}";
        private const string InsitutionIdTag = "{InstitutionId}";
        private string _goCardlessSettingsIndex = $"GocardlessBankInfo:{BankInfoTag}";
        private const string WebhookSecretKey = "GocardlessWebhookSecret";
        private readonly IConfiguration _configuration;

        private ApiResponse _apiResponse;

        public BankAccountController(
            IUserRepository userRepository,
            IBankAccountRepository bankAccountRepository,
            IRepository<Bank> banksRepository,
            IBankInfoService bankInfoService,
            IOptions<BankInfoServiceOptions> bankInfoServiceOptions,
            IOptions<GocardlessBankInfoOptions> goCardlessOptions,
            IConfiguration configuration,
            IFinancialAssetRepository financialAssetRepository,
            IRepository<BankSubAccount> bankSubAccountRepository,
            IEqualityComparer<BankSubAccount> bankSubAccountEqualityComparer)
        {
            _userRepository = userRepository;
            _apiResponse = new();
            _bankAccountRepository = bankAccountRepository;
            _banksRepository = banksRepository;
            _bankInfoService = bankInfoService;

            _bankInfoServiceOptions = bankInfoServiceOptions.Value;
            _goCardlessOptions = goCardlessOptions.Value;
            _configuration = configuration;
            _financialAssetRepository = financialAssetRepository;
            _bankSubAccountRepository = bankSubAccountRepository;
            _bankSubAccountEqualityComparer = bankSubAccountEqualityComparer;
        }


        [HttpPost(nameof(CreateAuthorisationLink))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<ApiResponse>> CreateAuthorisationLink([FromBody] AuthorisationLinkCreateDTO authorisationCreateDTO)
        {
            try
            {
                var user = await _userRepository.GetUserAsync(authorisationCreateDTO.UsernameOrEmail);

                if (user is null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.AddErrorMessage($"the username or email '{authorisationCreateDTO.UsernameOrEmail}' is invalid");
                    return BadRequest(_apiResponse);
                }

                var bank = await _banksRepository.GetAsync(
                    filter: (x) => x.BankName.ToUpper() == authorisationCreateDTO.BankName.ToUpper(),
                    tracked: false);

                if (bank is null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.AddErrorMessage($"the bank '{authorisationCreateDTO.BankName}' does'nt exists in the database");
                    return BadRequest(_apiResponse);
                }

                var bankAccount = await _bankAccountRepository.GetWithEntitiesAsync(
                    (x) => x.UserID == user.Id
                    && x.Bank!.BankID == bank.BankID, tracked: true);

                if (bankAccount is not null && bankAccount.IsAccessGranted)
                {

                    if (DateTime.Compare(
                        bankAccount.AccessGrantedAt.AddDays(bankAccount.AccessDurationInDays),
                        DateTime.UtcNow) >= 0)
                    {
                        _apiResponse.IsSuccess = false;
                        _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                        _apiResponse.AddErrorMessage($"the access to the '{bank.BankName}' account is still valid for the user '{user.UserName}'");
                        return BadRequest(_apiResponse);
                    }
                }

                string institutionId = _goCardlessOptions.BankInfos[bank.BankName].InstitutionId;

                var authorisationRedirectUrl =
                    $"https://localhost:7164/api/BankAccount/{nameof(AuthorizationCallback)}";

                var bankRequisition = await _bankInfoService.GetRequisition(
                    _bankInfoServiceOptions.ApiKey,
                    _bankInfoServiceOptions.ApiSecret,
                    institutionId,
                    authorisationRedirectUrl
                );
                
                if (bankRequisition is null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.AddErrorMessage($"the system could'nt create the authorisation link, please retry later");
                    return BadRequest(_apiResponse);
                }


                if (bankAccount is not null)
                {
                    await _bankAccountRepository.UpdateAsync(bankAccount, bankRequisition);
                }
                else
                {
                    await _bankAccountRepository.CreateAsync(new BankAccount()
                    {
                        UserID = user.Id,
                        RequisitionId = bankRequisition.Id,
                        RequisitionCreatedAt = DateTime.UtcNow,
                        BankId = bank.BankID,
                        AccessDurationInDays = _goCardlessOptions.BankInfos[bank.BankName].AccessDurationInDays
                    });
                }

                _apiResponse.Result = new AuthorisationLinkDTO()
                {
                    AuthorisationLink = bankRequisition.Link
                };

                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode = HttpStatusCode.Created;

                return CreatedAtAction(nameof(CreateAuthorisationLink), _apiResponse);

            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.AddErrorMessage(ex.Message);
                return _apiResponse;
            }

        }
        [HttpGet(nameof(AuthorizationCallback))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> AuthorizationCallback(
            [FromQuery(Name = "ref")] string referenceId)
        {

            try
            {
                var bankAccount = await _bankAccountRepository.GetWithEntitiesAsync(
                    x => x.RequisitionId == referenceId, tracked: true);

                if (bankAccount is null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.AddErrorMessage($"The reference Id '{referenceId}' doesn't exists in the database");
                    return BadRequest(_apiResponse);
                }

                await _bankAccountRepository.UpdateAsync(bankAccount, isAccessGranted: true);

                await GetSubAccounts(bankAccount);

                var financialAssets = await _financialAssetRepository.GetAllWithEntitiesAsync(
                    x => x.UserID == x.UserID &&
                    x.AssetPlatform!.Bank!.BankName == bankAccount!.Bank!.BankName);

                foreach(var financialAsset in financialAssets)
                {
                    await _financialAssetRepository.UpdateAsync(financialAsset: financialAsset, isAccountExists: true);
                }

                _apiResponse.IsSuccess = true;
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



        [HttpGet(nameof(GetAccessDuration))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> GetAccessDuration(
            [Required] string usernameOrEmail,
            [Required] string bankName)
        {

            try
            {
                var user = await _userRepository.GetUserAsync(usernameOrEmail);

                if (user is null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.AddErrorMessage($"the username or email '{usernameOrEmail}' is invalid");
                    return BadRequest(_apiResponse);
                }

                var bank = await _banksRepository.GetAsync(
                    filter: (x) => x.BankName.ToUpper() == bankName.ToUpper(),
                    tracked: false);

                if (bank is null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.AddErrorMessage($"the bank '{bankName}' does'nt exists in the database");
                    return BadRequest(_apiResponse);
                }


                var bankAccount = await _bankAccountRepository.GetWithEntitiesAsync(
                    (x) => x.UserID == user.Id
                    && x.Bank!.BankID == bank.BankID);

                if (bankAccount is null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.AddErrorMessage($"the user '{user.UserName}' does'nt have a '{bank.BankName}' account registered");
                    return BadRequest(_apiResponse);
                }

                _apiResponse.Result = new BankAccessDurationDTO()
                {
                    AccesssDurationInDays = 
                        (bankAccount.AccessGrantedAt.AddDays(bankAccount.AccessDurationInDays) - DateTime.UtcNow).TotalDays
                };

                _apiResponse.IsSuccess = true;
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

        private async Task GetSubAccounts(BankAccount bankAccount)
        {
            List<BankSubAccount> bankSubAccounts = new List<BankSubAccount>();

            var subAccountIds = await _bankInfoService.GetSubAccounts(
                _bankInfoServiceOptions.ApiKey,
                _bankInfoServiceOptions.ApiSecret,
                bankAccount.RequisitionId);

            if (subAccountIds is null)
                return;

            foreach (var subAccountId in subAccountIds)
            {
                var newSubAccount = new BankSubAccount()
                {
                    BankAccountID = bankAccount.BankAccountId,
                    BankSubAccountID = subAccountId
                };


                if (bankAccount.BankSubAccounts is not null &&
                    !bankAccount.BankSubAccounts.Contains(newSubAccount, _bankSubAccountEqualityComparer))
                {
                    await _bankSubAccountRepository.CreateAsync(newSubAccount);
                }



            }
        }

        [HttpDelete(nameof(Delete))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<ApiResponse>> Delete(
            [Required] string usernameOrMail,
            [Required] string bankName)
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

                var bank = await _banksRepository.GetAsync(
                    filter: (x) =>  x.BankName.ToUpper() == bankName.ToUpper(),
                    tracked: false);

                if (bank is null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.AddErrorMessage($"the bank '{bankName}' does'nt exists in the database");
                    return BadRequest(_apiResponse);
                }

                var bankAccount = await _bankAccountRepository.GetWithEntitiesAsync(
                    filter: (x) => x.UserID == user.Id && x.Bank!.BankName == bankName,
                    tracked: false);

                if (bankAccount is null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.AddErrorMessage($"the user '{user.UserName}' doesn't have a '{bankName}' account in the database");
                    return BadRequest(_apiResponse);
                }

                await _bankAccountRepository.RemoveAsync(bankAccount);

                var financialAssets = await _financialAssetRepository.GetAllWithEntitiesAsync(
                    x => x.UserID == user.Id && 
                    x.AssetPlatform!.Bank!.BankName == bankName);


                foreach(var asset in financialAssets)
                {
                    await _financialAssetRepository.UpdateAsync(financialAsset:asset, isAccountExists: false);
                }

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
