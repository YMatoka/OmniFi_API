using OmniFi_API.Models.Api;

namespace OmniFi_API.Services.Interfaces
{
    public interface IBaseService
    {
        public Task<T?> SendAsync<T>(ApiRequest apiRequest);
    }
}
