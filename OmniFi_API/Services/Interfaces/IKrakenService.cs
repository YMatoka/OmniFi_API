using OmniFi_API.Models.Portfolio;

namespace OmniFi_API.Services.Interfaces
{
    public interface IKrakenService
    {
        public Task<IEnumerable<PortfolioData>?> GetUserBalanceAsync(string ApiKey, string ApiSecret);
    }
}
