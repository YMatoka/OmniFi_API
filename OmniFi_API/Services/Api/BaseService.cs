using Azure;
using Newtonsoft.Json;
using Npgsql.Internal;
using OmniFi_API.Models.Api;
using OmniFi_API.Services.Interfaces;
using OmniFi_API.Utilities;
using System.Text;
using System.Web;
using static OmniFi_API.Utilities.ApiTypes;

namespace OmniFi_API.Services.Api
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClient;
        private const string DefaultMediaType = MediaTypes.JsonMediaType;

        public BaseService(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<T?> SendAsync<T>(ApiRequest apiRequest)
        {
            try
            {
                using (var client = _httpClient.CreateClient())
                {

                    var mediaType = apiRequest.MediaType is not null ? 
                        apiRequest.MediaType : 
                        DefaultMediaType;

                    StringContent? encodedData = null;

                    if (apiRequest.Data is not null)
                    {
                        switch (mediaType)
                        {
                            case MediaTypes.WwwFormMediaType:
                                encodedData = new StringContent(
                                    (string)apiRequest.Data,
                                    Encoding.UTF8,
                                    MediaTypes.WwwFormMediaType);
                                break;

                            case MediaTypes.JsonMediaType:
                                encodedData = new StringContent(
                                    JsonConvert.SerializeObject(apiRequest.Data), 
                                    Encoding.UTF8, 
                                    MediaTypes.JsonMediaType);
                                break;
                        }
                    }

                    var request = new HttpRequestMessage(HttpMethod.Post, apiRequest.Url);

                    HttpRequestMessage message = new HttpRequestMessage();

                    if (apiRequest.HeaderDictionary is not null)
                    {
                        foreach (var keyValuePair in apiRequest.HeaderDictionary)
                        {
                            message.Headers.Add(keyValuePair.Key, keyValuePair.Value);
                        }

                    }

                    message.RequestUri = new Uri(apiRequest.Url);

                    if (encodedData is not null)        
                        message.Content = encodedData;

                    message.Method = GetRequestMethodType(apiRequest.ApiType);

                    HttpResponseMessage? httpResponseMessage = null;

                    httpResponseMessage = await client.SendAsync(message);

                    var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();

                    if (httpResponseMessage.StatusCode != System.Net.HttpStatusCode.OK)
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
