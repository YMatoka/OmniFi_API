using OmniFi_API.Models.Assets;
using System.Linq.Expressions;

namespace OmniFi_API.Repository.Interfaces
{
    public interface IFinancialAssetHistoryRepository : IRepository<FinancialAssetHistory>
    {
        Task<IEnumerable<FinancialAssetHistory>> GetAllWithEntitiesAsync(Expression<Func<FinancialAssetHistory, bool>>? filter = null, bool tracked = false);

    }
}
