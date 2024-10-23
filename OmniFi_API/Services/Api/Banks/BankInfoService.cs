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
using AutoMapper;
using Microsoft.Identity.Client;

namespace OmniFi_API.Services.Api.Banks
{
    public class BankInfoService : BaseService, IBankInfoService, IFinancialAssetService
    {
        private readonly BankInfoServiceOptions _options;

        private const string DefaultBankInfoBaseUrl = "https://bankaccountdata.gocardless.com";
        private const string DefaultRedirectUrl = "https://www.google.com";

        private const string SearchedBalanceType = "closingBooked";

        private readonly IRepository<BankAgreement> _bankAgreementRepository;
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly IRepository<BankSubAccount> _bankSubAccountRepository;

        private readonly IBankDataApiRepository _bankApiCredentialRepository;
        private readonly IMapper _mapper;
        private readonly string _mediaFormat = MediaTypes.JsonMediaType;

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
        private const string DefaultCountryCode = "FR";

        private const string AgreementMethod = "/api/v2/agreements/enduser/";
        private const int AccessDurationInDays = 90;

         
        private const string RequistitionMethod = "/api/v2/requisitions/";
        private const string RequisitionIdTag = "{RequisitionIdTag}";
        private const string AccountsListingMethod = $"{RequistitionMethod}{RequisitionIdTag}/" ;

        private const string AccountIdTag = "{AccountIdTag}";
        private const string BalancesMethod = $"/api/v2/accounts/{AccountIdTag}/balances/";

        private readonly string _bankInfoServiceBaseUrl;
        public BankInfoService(IHttpClientFactory httpClientFactory,
            IOptions<BankInfoServiceOptions> options,
            IConfiguration configuration,
            IBankDataApiRepository bankApiCredentialRepository,
            IRepository<BankAgreement> bankAgreementRepository,
            IBankAccountRepository bankAccountRepository,
            IRepository<BankSubAccount> bankSubAccount,
            IMapper mapper) : base(httpClientFactory)
        {
            _options = options.Value;
            _bankInfoServiceBaseUrl = _options.ApiBaseUrl ?? DefaultBankInfoBaseUrl;
            _bankApiCredentialRepository = bankApiCredentialRepository;
            _bankAgreementRepository = bankAgreementRepository;
            _bankAccountRepository = bankAccountRepository;
            _bankSubAccountRepository = bankSubAccount;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PortfolioData>?> GetUserBalanceAsync(
            string apiKey, 
            string apiSecret,
            string? accountId,
            string? platformName)
        {

            List<PortfolioData> portfolioDatas = new List<PortfolioData>();

            var accessToken = await GetAcessToken(apiKey, apiSecret);

            if (platformName is null)
            {
                throw new ArgumentNullException("the platformName variable is null");
            }

            if (accountId is null)
            {
                throw new ArgumentNullException("the accountId variable is null");
            }

            if (accessToken is null)
            {
                throw new Exception("the access token is null");
            }

            var balancesResponse = await SendAsync<BalancesResponse>(new ApiRequest()
            {
                ApiType = ApiTypes.ApiType.GET,
                Url = _bankInfoServiceBaseUrl + BalancesMethod.Replace(AccountIdTag, accountId),
                HeaderDictionary = new Dictionary<string, string>()
                {
                    [AcceptHeaderKey] = _mediaFormat,
                    [AuthorisationHeaderKey] = AuthorisationHeaderItem.Replace(AccesTokenTag, accessToken)
                }
            });

            if(balancesResponse is null)
                return null;

            // map gocardless account type to the programe account type
            foreach (var balance in balancesResponse.balances)
            {
                if(!(balance.balanceType.ToLower() == SearchedBalanceType.ToLower()))
                        continue;

                portfolioDatas.Add(new PortfolioData() { 
                    AssetPlatformName = platformName,
                    AssetSourceName = AssetSourceNames.CheckingAccount,
                    Balance = decimal.Parse(balance.balanceAmount.amount),
                    FiatCurrencyCode = balance.balanceAmount.currency.ToUpper()
                });
            }

            return portfolioDatas;
        }


        // call it in the webhook controller 
        public async Task<IEnumerable<string>?> GetSubAccounts(
            string apiKey, 
            string apiSecret,
            string requisitionId)
        {
            var accessToken = await GetAcessToken(apiKey, apiSecret);

            if (accessToken is null)
            {
                throw new Exception("the access token is null");
            }

            var subAccounts = await SendAsync<RequisitionResponse>(new ApiRequest()
            { 
                ApiType = ApiTypes.ApiType.GET,
                Url = _bankInfoServiceBaseUrl + AccountsListingMethod.Replace(RequisitionIdTag, requisitionId),
                HeaderDictionary = new Dictionary<string, string>()
                {
                    [AcceptHeaderKey] = _mediaFormat,
                    [AuthorisationHeaderKey] = AuthorisationHeaderItem.Replace(AccesTokenTag, accessToken)
                }

            });

            return subAccounts?.accounts;

        }



        public async Task<BankRequisition?> GetRequisition(
            string apiKey,
            string apiSecret,
            string institutionId,
            string? redirectUrl = null)
        {
            var accessToken = await GetAcessToken(apiKey, apiSecret);

            if (accessToken is null)
            {
                throw new Exception("the access token is null");
            }

            if (string.IsNullOrEmpty(redirectUrl))
            {
                redirectUrl = DefaultRedirectUrl;
            }

            var requisitionResponse = await BuildRequisition(accessToken, redirectUrl, institutionId);

            return _mapper.Map<BankRequisition>(requisitionResponse);
         
        }

        private async Task<RequisitionResponse?> BuildRequisition(string accessToken, string redirectUrl, string institutionId)
        {
            return await SendAsync<RequisitionResponse>(new ApiRequest()
            {
                ApiType = ApiTypes.ApiType.POST,
                Url = _bankInfoServiceBaseUrl + Endpoint.Replace(MethodTag, RequistitionMethod),
                HeaderDictionary = new Dictionary<string, string>()
                {
                    [AcceptHeaderKey] = _mediaFormat,
                    [AuthorisationHeaderKey] = AuthorisationHeaderItem.Replace(AccesTokenTag, accessToken),
                },
                Data = new Dictionary<string, string>()
                {
                    ["redirect"] = redirectUrl,
                    ["institution_id"] = institutionId,
                    ["user_language"] = DefaultCountryCode,
                }
            });
        }

        private async Task<string?> GetAcessToken(string apiKey, string apiSecret)
        {
            var bankApiCredential = await _bankApiCredentialRepository
                .GetAsync(tracked: true);

            if (bankApiCredential is null||
                (DateTime.Compare(
                    bankApiCredential.AccessTokenCreatedAt.AddSeconds(bankApiCredential.AccessExpires),
                    DateTime.UtcNow
                ) <= 0
                &&
                    DateTime.Compare(
                    bankApiCredential.RefreshokenCreatedAt.AddSeconds(bankApiCredential.RefreshExpires),
                    DateTime.UtcNow
                ) <= 0))
            {

                var response =  await SendAsync<AccessTokenResponse>(new ApiRequest()
                {
                    ApiType = ApiTypes.ApiType.POST,
                    Url = _bankInfoServiceBaseUrl + Endpoint.Replace(MethodTag, UserTokenMethod),
                    HeaderDictionary = new Dictionary<string, string>
                    {
                        [AcceptHeaderKey] = _mediaFormat,
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
                    DateTime.UtcNow
                ) <= 0 
                &&
                DateTime.Compare(
                    bankApiCredential.RefreshokenCreatedAt.AddSeconds(bankApiCredential.RefreshExpires),
                    DateTime.UtcNow
                ) > 0)
            {

                var response = await SendAsync<RefreshTokenResponse>(new ApiRequest()
                {
                    ApiType = ApiTypes.ApiType.POST,
                    Url = _bankInfoServiceBaseUrl + Endpoint.Replace(MethodTag, UserRefreshTokenMethod),
                    HeaderDictionary = new Dictionary<string, string>
                    {
                        [AcceptHeaderKey] = _mediaFormat,
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
                    AccessTokenCreatedAt = DateTime.UtcNow,
                    RefreshokenCreatedAt = DateTime.UtcNow
                });

            }
            else if (bankDataApiCredential is not null)
            {
                bankDataApiCredential.AccessToken = accessTokenResponse.access;
                bankDataApiCredential.AccessExpires = accessTokenResponse.access_expires;
                bankDataApiCredential.RefreshToken = accessTokenResponse.refresh;
                bankDataApiCredential.RefreshExpires = accessTokenResponse.refresh_expires;
                bankDataApiCredential.AccessTokenCreatedAt = DateTime.UtcNow;
                bankDataApiCredential.RefreshokenCreatedAt = DateTime.UtcNow;

                await _bankApiCredentialRepository.UpdateAsync(bankDataApiCredential);
            }

            
        }
        private async Task StoreAccessTokenResponse(RefreshTokenResponse refreshTokenResponse, BankDataApiCredential bankDataApiCredential)
        {

            bankDataApiCredential.AccessToken = refreshTokenResponse.access;
            bankDataApiCredential.AccessExpires = refreshTokenResponse.access_expires;
            bankDataApiCredential.AccessTokenCreatedAt = DateTime.UtcNow;

            await _bankApiCredentialRepository.UpdateAsync(bankDataApiCredential);

        }
    }
}
