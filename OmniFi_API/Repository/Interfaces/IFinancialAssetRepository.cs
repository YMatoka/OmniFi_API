using OmniFi_API.DTOs.CoinMarketCap;
using OmniFi_API.Models.Abstracts;
using OmniFi_API.Models.Assets;
using OmniFi_API.Models.Portfolio;
using System.Linq.Expressions;

namespace OmniFi_API.Repository.Interfaces
{
    public interface IFinancialAssetRepository: IRepository<FinancialAsset>
    {
        Task<IEnumerable<FinancialAsset>> GetAllWithEntitiesAsync(Expression<Func<FinancialAsset, bool>>? filter = null, bool tracked = false);
        Task<FinancialAsset?> GetWithEntitiesAsync(Expression<Func<FinancialAsset, bool>>? filter = null, bool tracked = false);
        Task CreateAsync(FinancialAsset financialAsset, PortfolioData portfolioData);
        Task UpdateAsync(FinancialAsset financialAsset, PortfolioData portfolioData);
        Task UpdateAsync(FinancialAsset financialAsset, bool isAccountExists);
    }

}
