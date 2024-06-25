using Microsoft.EntityFrameworkCore;
using OmniFi_API.Data;
using OmniFi_API.Models.Abstracts;
using OmniFi_API.Models.Assets;
using OmniFi_API.Repository.Interfaces;
using System.Linq.Expressions;

namespace OmniFi_API.Repository.Assets
{
    public class FinancialAssetRepository : BaseRepository<FinancialAsset>, IFinancialAssetRepository
    {


        public FinancialAssetRepository(ApplicationDbContext db) : base(db)
        {
            
        }

        public async Task UpdateAsync(FinancialAsset financialAsset)
        {
            db.Update(financialAsset);
            await SaveAsync();
        }

        public async Task<FinancialAsset?> GetWithEntitiesAsync(Expression<Func<FinancialAsset, bool>>? filter = null, bool tracked = false)
        {
            IQueryable<FinancialAsset> query = _dbSet;

            if (!tracked)
            {
                query.AsNoTracking();
            }

            if(filter is not null)
            {
                query = query
                    .Where(filter);
            }

            await query
                .Include(x => x.AssetSource)
                .Include(x => x.User)
                .Include(x => x.AssetPlatform)
                    .ThenInclude(x => x!.Bank)
                .Include(x => x.AssetPlatform)
                    .ThenInclude(x => x!.CryptoExchange)
                .Include(x => x.CryptoHolding)
                .Include(x => x.FiatCurrency)
                .LoadAsync();

            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<FinancialAsset>> GetAllWithEntitiesAsync(Expression<Func<FinancialAsset, bool>>? filter = null , bool tracked = false)
        {

            IQueryable<FinancialAsset> query = _dbSet;

            if (!tracked)
            {
                query.AsNoTracking();
            }

            if(filter is not null)
            {
                query = query.Where(filter);    
            }

            await query
                .Include(x => x.AssetSource)
                .Include(x => x.User)
                .Include(x => x.AssetPlatform)
                    .ThenInclude(x => x!.Bank)
                .Include(x => x.AssetPlatform)
                    .ThenInclude(x => x!.CryptoExchange)
                .Include(x => x.CryptoHolding)
                .Include(x => x.FiatCurrency)
                .LoadAsync();
                
           return await query.ToListAsync();

        }
    }
}
