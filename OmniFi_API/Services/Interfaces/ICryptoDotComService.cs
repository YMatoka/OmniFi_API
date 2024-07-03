using OmniFi_API.DTOs.CryptoDotCom;

namespace OmniFi_API.Services.Interfaces
{
    public interface ICryptoDotComService
    {
        public Task<UserBalanceResponse?> GetUserBalanceAsync(string ApiKey, string ApiSecret);
    }
}
