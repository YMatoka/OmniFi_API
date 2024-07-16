using Microsoft.Extensions.Options;
using OmniFi_API.DTOs.CoinMarketCap;
using OmniFi_API.Models.Api;
using OmniFi_API.Options.Cryptos;
using OmniFi_API.Services.Interfaces;
using static OmniFi_API.Utilities.ApiTypes;

namespace OmniFi_API.Services.Api.Cryptos
{
    public class CryptoInfoService : BaseService, ICryptoInfoService
    {
        private readonly CryptoInfoServiceOptions _options;

        private const string VersionTag = "{version}";
        private const string MethodTag = "{method}";
        private const string Endpoint = $"/{VersionTag}/{MethodTag}";

        private const string CryptoInfoMethod = "cryptocurrency/info";
        private const string ApiVersion = "v2";

        private const string ApiHeaderKey = "X-CMC_PRO_API_KEY";
        private const string ApiQueryParameter = "CMC_PRO_API_KEY";
        private const string SymbolQueryParameterKey = "symbol";


        public CryptoInfoService(
            IHttpClientFactory httpClientFactory,
            IOptions<CryptoInfoServiceOptions> options) : base(httpClientFactory)
        {
            _options = options.Value;
        }

        public async Task<IEnumerable<CryptoInfo>?> GetAllCryptoInfos(IEnumerable<string> cryptoSymbolList)
        {
            var cryptoInfoResponse = await SendAsync<CryptoInfoResponse>(new ApiRequest()
            {
                ApiType = ApiType.GET,
                HeaderDictionary = new Dictionary<string, string> { { ApiHeaderKey, _options.ApiKey } },
                Url = GetUrlWithQueryParameters(cryptoSymbolList)
            });

            return cryptoInfoResponse is not null ?
                ParseCryptoInfo(cryptoInfoResponse) : 
                null;
        }

        private IEnumerable<CryptoInfo> ParseCryptoInfo(CryptoInfoResponse cryptoInfoResponse)
        {
            List<CryptoInfo> result = new List<CryptoInfo>();

            foreach (var keyValuePair in cryptoInfoResponse.data)
            {
                result.Add(keyValuePair.Value.First());
            }

            return result;
        }

        private string GetUrlWithQueryParameters(IEnumerable<string> cryptoSymbolList)
        {
            var queryParameters = Uri.EscapeDataString(string.Join(_options.Separator, cryptoSymbolList));

            return _options.ApiBaseUrl + Endpoint
                .Replace(VersionTag, ApiVersion)
                .Replace(MethodTag, CryptoInfoMethod)
                + "?"
                + SymbolQueryParameterKey + "=" + queryParameters;
 
        }
    }
}
