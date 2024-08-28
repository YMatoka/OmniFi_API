using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using OmniFi_API.Options.Banks;
using OmniFi_API.Utilities;

namespace OmniFi_API.Services.Api.Banks
{
    public class BankInfoService
    {
        private readonly BankInfoServiceOptions _options;

        private const string DefaultBankInfoBaseUrl = "https://bankaccountdata.gocardless.com";

        private const string AcceptHeaderKey = "accept";
        private const string ContentTyoeHeaderKey = "Content-Type";

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
        

        public BankInfoService(IOptions<BankInfoServiceOptions> options)
        {
            _options = options.Value;
        }
    }
}
