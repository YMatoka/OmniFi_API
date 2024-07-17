using Microsoft.EntityFrameworkCore;
using OmniFi_API.Data;
using OmniFi_API.Models.Assets;
using OmniFi_API.Repository.Interfaces;
using System.Linq;
using System.Linq.Expressions;

namespace OmniFi_API.Repository.Assets
{
    public class FinancialAssetHistoryRepository : BaseRepository<FinancialAssetHistory>, IFinancialAssetHistoryRepository
    {
        public FinancialAssetHistoryRepository(ApplicationDbContext db) : base (db)
        {
            
        }

        public async Task<IEnumerable<FinancialAssetHistory>> GetAllWithEntitiesAsync(Expression<Func<FinancialAssetHistory, bool>>? filter = null, bool tracked = false)
        {
        
            IQueryable<FinancialAssetHistory> query = _dbSet;

            if (!tracked)
            {
                query.AsNoTracking();
            }

            if (filter is not null)
            {
                query = query.Where(filter);
            }

            await query
                .Include(x => x.FinancialAsset)
                .Include(x => x.AssetSource)
                .Include(x => x.User)
                .Include(x => x.AssetPlatform)
                    .ThenInclude(x => x!.Bank)
                .Include(x => x.AssetPlatform)
                    .ThenInclude(x => x!.CryptoExchange)
                .Include(x => x.CryptoHoldingHistory)
                    .ThenInclude(x => x!.CryptoCurrency)
                .Include(x => x.FiatCurrency)
                .LoadAsync();

            return await query.ToListAsync();
        }
    }
}
