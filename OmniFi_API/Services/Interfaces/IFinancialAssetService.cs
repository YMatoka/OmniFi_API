using OmniFi_API.Models.Portfolio;

namespace OmniFi_API.Services.Interfaces
{
    public interface IFinancialAssetService
    {
        public Task<IEnumerable<PortfolioData>?> GetUserBalanceAsync(string ApiKey, string ApiSecret);
    }
}
