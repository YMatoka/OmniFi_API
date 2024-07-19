using Newtonsoft.Json;
using Npgsql.Internal;
using OmniFi_API.Models.Api;
using OmniFi_API.Services.Interfaces;
using OmniFi_API.Utilities;
using System.Text;
using static OmniFi_API.Utilities.ApiTypes;

namespace OmniFi_API.Services.Api
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClient;
        private const string DefaultApiContentType = "application/json";

        public BaseService(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient; 
        }

        public async Task<T?> SendAsync<T>(ApiRequest apiRequest)
        {
            try
            {
                using( var client = _httpClient.CreateClient())
                {
                    HttpRequestMessage message = new HttpRequestMessage();
                    message.Headers.Add("Accept", DefaultApiContentType);

                    if(apiRequest.HeaderDictionary is not null)
                    {
                        foreach(var keyValuePair in apiRequest.HeaderDictionary)
                        {
                            message.Headers.Add(keyValuePair.Key, keyValuePair.Value);
                        }
                       
                    }

                    message.RequestUri = new Uri(apiRequest.Url);

                    if(apiRequest.Data is not null)
                    {
                        message.Content = new StringContent(
                            JsonConvert.SerializeObject(apiRequest.Data),
                            Encoding.UTF8,
                            DefaultApiContentType);
                    }

                    message.Method = GetRequestMethodType(apiRequest.ApiType);

                    HttpResponseMessage? httpResponseMessage = null;

                    httpResponseMessage = await client.SendAsync(message);
                    
                    var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();

                    if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        throw new Exception(responseContent);
                    }

                    var deserializeContent = JsonConvert.DeserializeObject<T>(responseContent);

                    return deserializeContent;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private HttpMethod GetRequestMethodType(ApiType apiType)
        {
            switch (apiType)
            {
                case ApiType.GET:
                    return HttpMethod.Get;
                case ApiType.POST:
                    return HttpMethod.Post;
                case ApiType.PUT:
                    return HttpMethod.Put;
                case ApiType.DELETE:
                    return HttpMethod.Delete;
                default:
                    return HttpMethod.Get;
            }
        }
    }
}
