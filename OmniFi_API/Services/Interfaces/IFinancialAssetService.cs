using OmniFi_API.Models.Portfolio;

namespace OmniFi_API.Services.Interfaces
{
    public interface IFinancialAssetService
    {
        public Task<IEnumerable<PortfolioData>?> GetUserBalanceAsync(string apiKey, string apiSecret, string? accountId = null, string? platformName = null);
    
    }
}
