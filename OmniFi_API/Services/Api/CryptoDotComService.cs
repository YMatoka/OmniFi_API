using OmniFi_API.DTOs.CryptoDotCom;
using OmniFi_API.Services.Interfaces;
using OmniFi_API.Models.Api;
using OmniFi_API.Models.Api.CryptoDotCom;
using System.Security.Cryptography;
using System.Text;
using NuGet.Common;
using static OmniFi_API.Utilities.ApiTypes;

namespace OmniFi_API.Services.Api
{
    public class CryptoDotComService : BaseService, ICryptoDotComService
    {
        private const string DefaultBaseUrl = "https://api.crypto.com/exchange";
        private const string ConfigBaseUrlIndex = "ApiBaseUrls:CryptoDotCom";

        private readonly string _cryptoDotComBaseUrl;

        private const string VersionTag = "{version}";
        private const string MethodTag = "{method}";
        private const string Endpoint = $"/{VersionTag}/{MethodTag}";

        private const string UserBalanceMethod = "private/user-balance";
        private const string ApiVersion = "v1";


        public CryptoDotComService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient)
        {
            _cryptoDotComBaseUrl = configuration.GetValue<string>(ConfigBaseUrlIndex) ?? DefaultBaseUrl;
        }

        public Task<UserBalanceResponse?> GetUserBalanceAsync(string apiKey, string apiSecret)
        {
            var cryptoDotComRequest = new CryptoDotComRequest()
            {
                Id = 7,
                Method = UserBalanceMethod,
                Api_key = apiKey,
                Nonce = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

            cryptoDotComRequest.Sig = GetSign(cryptoDotComRequest, apiKey, apiSecret);

            return SendAsync<UserBalanceResponse>(new ApiRequest()
            {
                ApiType = ApiType.POST,

                Url = _cryptoDotComBaseUrl + Endpoint
                .Replace(VersionTag, ApiVersion)
                .Replace(MethodTag, UserBalanceMethod),

                Data = cryptoDotComRequest
            });
        }

        private string GetSign(CryptoDotComRequest request, string apiKey, string apiSecret)
        {
            string paramString = string.Join("",
                request.Params.Keys.OrderBy(key => key)
                .Select(key => key + request.Params[key]));

            string sigPayLoad = request.Method + request.Id + apiKey + paramString + request.Nonce;

            var hash = new HMACSHA256(Encoding.Unicode.GetBytes(apiSecret));

            var computedHash = hash.ComputeHash(Encoding.Unicode.GetBytes(sigPayLoad));

            return Convert.ToHexString(computedHash);
        }
    }
}
