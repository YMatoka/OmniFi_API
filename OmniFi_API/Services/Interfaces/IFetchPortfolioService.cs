using OmniFi_API.Models.Identity;

namespace OmniFi_API.Services.Interfaces
{
    public interface IFetchPortfolioService
    {
        public Task FetchPortfolio(string user, string? bankName = null, string? cryptoExchangeName = null);
    }
}
