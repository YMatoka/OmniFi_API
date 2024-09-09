using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using OmniFi_API.DTOs.GoCardless;
using OmniFi_API.Models.Api.Banks;
using OmniFi_API.Models.Banks;
using OmniFi_API.Models.Portfolio;
using OmniFi_API.Options.Banks;
using OmniFi_API.Repository.Interfaces;
using OmniFi_API.Services.Interfaces;
using OmniFi_API.Utilities;
using OmniFi_API.Models.Api;
using Azure.Core;
using Azure;

namespace OmniFi_API.Services.Api.Banks
{
    public class BankInfoService : BaseService, IFinancialAssetService
    {
        private readonly BankInfoServiceOptions _options;
        private const string DefaultBankInfoBaseUrl = "https://bankaccountdata.gocardless.com";


        private readonly IRepository<BankAgreement> _bankAgreementRepository;
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly IRepository<BankSubAccount> _bankSubAccountRepository;

        private readonly IBankDataApiRepository _bankApiCredentialRepository;
        private readonly string mediaFormat = MediaTypes.JsonMediaType;

        private const string AcceptHeaderKey = "accept";
        private const string ContentTypeHeaderKey = "Content-Type";

        private const string AccesTokenTag = "{AccessTokenTag}";

        private const string AuthorisationHeaderKey = "Authorization";
        private const string AuthorisationHeaderItem= $"Bearer {AccesTokenTag}";

        private const string MethodTag = "{method}";
        private const string Endpoint = $"{MethodTag}";

        private const string UserTokenMethod = "/api/v2/token/new/";
        private const string UserRefreshTokenMethod = "/api/v2/token/refresh/";

        private const string BankListMethod = "/api/v2/institutions/";
        private const string CountryParamater = "country";
        private const string DefaultCountryCOde = "fr";

        private const string AgreementMethod = "/api/v2/agreements/enduser/";
        private const int AccessDurationInDays = 90;

        private Dictionary<string, string> _aggreementData = new Dictionary<string, string>()
        {
            ["institution_id"] = string.Empty,
            ["max_historical_days"] = string.Empty,
            ["agreement"] = string.Empty,
            ["access_scope"] = string.Empty,
        };

        private const string RequistitionMethod = "/api/v2/requisitions/";
        private const string DefaultRedirectUrl = "";
        private Dictionary<string, string> _requisitionData = new Dictionary<string, string>()
        {
            ["redirect"] = string.Empty,
            ["institution_id"] = string.Empty,
            ["reference"] = string.Empty,
            ["agreement"] = string.Empty,
            ["user_language"] = string.Empty,
        };

        private const string RequisitionIdTag = "{RequisitionIdTag}";
        private const string AccountsListingMethod = $"{RequistitionMethod}{RequisitionIdTag}/" ;

        private const string AccountIdTag = "{AccountIdTag}";
        private const string AccountsDataMethod = $"/api/v2/accounts/{AccountIdTag}/";

        private readonly string _bankInfoServiceBaseUrl;
        public BankInfoService(IHttpClientFactory httpClientFactory, 
            IOptions<BankInfoServiceOptions> options, 
            IConfiguration configuration,
            IBankDataApiRepository bankApiCredentialRepository,
            IRepository<BankAgreement> bankAgreementRepository,
            IBankAccountRepository bankAccountRepository,
            IRepository<BankSubAccount> bankSubAccount) : base(httpClientFactory) 
        {
            _options = options.Value;
            _bankInfoServiceBaseUrl = _options.ApiBaseUrl ?? DefaultBankInfoBaseUrl;
            _bankApiCredentialRepository = bankApiCredentialRepository;
            _bankAgreementRepository = bankAgreementRepository;
            _bankAccountRepository = bankAccountRepository;
            _bankSubAccountRepository = bankSubAccount;
        }

        public async Task<IEnumerable<PortfolioData>?> GetUserBalanceAsync(
            string ApiKey, 
            string ApiSecret, 
            string UserId, 
            string bankName,
            string institutionId,
            string? redirectUrl = null)
        {
            var accessToken = await GetAcessToken(ApiKey, ApiSecret);

            if (accessToken == null)
                return null;

            // add related entities retrieving
            var bankAccount = await _bankAccountRepository
                .GetAsync(x => x.UserID == UserId && x.Bank.BankName == bankName);

            if(bankAccount is null ||
                DateTime.Compare(
                    bankAccount.RequisitionCreatedAt.AddDays(bankAccount.RequisitionDurationInDays), 
                    DateTime.Now) > 0)
            {
                await BuildLink(accessToken, redirectUrl, institutionId);
            }

        }

        private async Task BuildLink(string accessToken, string? redirectUrl, string institutionId)
        {
            throw new NotImplementedException();
        }

        private async Task<string?> GetAcessToken(string apiKey, string apiSecret)
        {
            var bankApiCredential = await _bankApiCredentialRepository
                .GetAsync(tracked: true);

            if (bankApiCredential is null||
                    DateTime.Compare(
                    bankApiCredential.RefreshokenCreatedAt.AddSeconds(bankApiCredential.RefreshExpires),
                    DateTime.Now
                ) >= 0)
            {

                var response =  await SendAsync<AccessTokenResponse>(new ApiRequest()
                {
                    ApiType = ApiTypes.ApiType.POST,
                    Url = _bankInfoServiceBaseUrl + Endpoint.Replace(MethodTag, UserTokenMethod),
                    HeaderDictionary = new Dictionary<string, string>
                    {
                        [AcceptHeaderKey] = mediaFormat,
                        [ContentTypeHeaderKey] = mediaFormat
                    },
                    Data = new Dictionary<string, string>()
                    {
                        ["secret_id"] = apiKey,
                        ["secret_key"] = apiSecret
                    }
                });

                if(response is not null)
                {
                    await StoreAccessTokenResponse(response, bankApiCredential);
                }

                return response?.access;
            }
            else if (
                DateTime.Compare(
                    bankApiCredential.AccessTokenCreatedAt.AddSeconds(bankApiCredential.AccessExpires),
                    DateTime.Now
                ) > 0 
                &&
                DateTime.Compare(
                    bankApiCredential.RefreshokenCreatedAt.AddSeconds(bankApiCredential.RefreshExpires),
                    DateTime.Now
                ) < 0)
            {

                var response = await SendAsync<RefreshTokenResponse>(new ApiRequest()
                {
                    ApiType = ApiTypes.ApiType.POST,
                    Url = _bankInfoServiceBaseUrl + Endpoint.Replace(MethodTag, UserRefreshTokenMethod),
                    HeaderDictionary = new Dictionary<string, string>
                    {
                        [AcceptHeaderKey] = mediaFormat,
                        [ContentTypeHeaderKey] = mediaFormat
                    },
                    Data = new Dictionary<string, string>()
                    {
                        ["refresh"] = bankApiCredential.RefreshToken,
                    }
                });

                if(response is not null)
                {
                    await StoreAccessTokenResponse(response, bankApiCredential);
                }

                return response?.access;
            }

            return bankApiCredential.AccessToken;
        }

        private async Task StoreAccessTokenResponse(AccessTokenResponse accessTokenResponse, BankDataApiCredential? bankDataApiCredential)
        {
            if (bankDataApiCredential is null)
            {
                await _bankApiCredentialRepository.CreateAsync(new BankDataApiCredential()
                {
                    AccessToken = accessTokenResponse.access,
                    AccessExpires = accessTokenResponse.access_expires,
                    RefreshToken = accessTokenResponse.refresh,
                    RefreshExpires = accessTokenResponse.refresh_expires,
                    AccessTokenCreatedAt = DateTime.Now,
                    RefreshokenCreatedAt = DateTime.Now
                });

            }
            else if (bankDataApiCredential is not null)
            {
                bankDataApiCredential.AccessToken = accessTokenResponse.access;
                bankDataApiCredential.AccessExpires = accessTokenResponse.access_expires;
                bankDataApiCredential.RefreshToken = accessTokenResponse.refresh;
                bankDataApiCredential.RefreshExpires = accessTokenResponse.refresh_expires;
                bankDataApiCredential.AccessTokenCreatedAt = DateTime.Now;
                bankDataApiCredential.RefreshokenCreatedAt = DateTime.Now;

                await _bankApiCredentialRepository.UpdateAsync(bankDataApiCredential);
            }

            
        }
        private async Task StoreAccessTokenResponse(RefreshTokenResponse refreshTokenResponse, BankDataApiCredential bankDataApiCredential)
        {

            bankDataApiCredential.AccessToken = refreshTokenResponse.access;
            bankDataApiCredential.AccessExpires = refreshTokenResponse.access_expires;
            bankDataApiCredential.AccessTokenCreatedAt = DateTime.Now;

            await _bankApiCredentialRepository.UpdateAsync(bankDataApiCredential);

        }
    }
}
