using OmniFi_API.DTOs.CryptoDotCom;
using OmniFi_API.Models.Portfolio;

namespace OmniFi_API.Services.Interfaces
{
    public interface ICryptoDotComService
    {
        public Task<IEnumerable<PortfolioData>?> GetUserBalanceAsync(string ApiKey, string ApiSecret);
    }
}
