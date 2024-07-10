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
        private readonly IFinancialAssetHistoryRepository _financialAssetHistoryRepository;

        public FinancialAssetRepository(ApplicationDbContext db, 
            IFinancialAssetHistoryRepository financialAssetHistoryRepository) : base(db)
        {
            _financialAssetHistoryRepository = financialAssetHistoryRepository;
        }

        public async Task UpdateAsync(FinancialAsset financialAsset, decimal newValue)
        {
            using(var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var actualDateTime = DateTime.UtcNow;

                    var financialAssetHistory = new FinancialAssetHistory()
                    {
                        UserID = financialAsset.UserID,
                        AssetPlatformID = financialAsset.AssetPlatformID,
                        AssetSourceID = financialAsset.AssetSourceID,
                        Value = financialAsset.Value,
                        FiatCurrencyID = financialAsset.FiatCurrencyID,
                        FinancialAssetId = financialAsset.FinancialEntityId,
                        RecordedAt = actualDateTime
                    };

                    financialAsset.LastUpdatedAt = actualDateTime;
                    financialAsset.Value = newValue;

                    await _financialAssetHistoryRepository.CreateAsync(financialAssetHistory);

                    db.Update(financialAsset);

                    await SaveAsync();

                    await transaction.CommitAsync();
                }
                catch (Exception)
                {

                    throw;
                }

            }
        }

        public async override Task CreateAsync(FinancialAsset financialAsset)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var financialAssetHistory = new FinancialAssetHistory()
                    {
                        UserID = financialAsset.UserID,
                        AssetPlatformID = financialAsset.AssetPlatformID,
                        AssetSourceID = financialAsset.AssetSourceID,
                        Value = financialAsset.Value,
                        FiatCurrencyID = financialAsset.FiatCurrencyID,
                        FinancialAssetId = financialAsset.FinancialEntityId,
                        RecordedAt = financialAsset.FirstRetrievedAt
                    };

                    await _financialAssetHistoryRepository.CreateAsync(financialAssetHistory);

                    await base.CreateAsync(financialAsset);

                    await transaction.CommitAsync();
                }
                catch (Exception)
                {

                    throw;
                }

            }
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
