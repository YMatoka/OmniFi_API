﻿using Microsoft.EntityFrameworkCore;
using OmniFi_API.Data;
using OmniFi_API.DTOs.CoinMarketCap;
using OmniFi_API.Models.Abstracts;
using OmniFi_API.Models.Assets;
using OmniFi_API.Models.Cryptos;
using OmniFi_API.Models.Portfolio;
using OmniFi_API.Repository.Cryptos;
using OmniFi_API.Repository.Interfaces;
using OmniFi_API.Services.Interfaces;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace OmniFi_API.Repository.Assets
{
    public class FinancialAssetRepository : BaseRepository<FinancialAsset>, IFinancialAssetRepository
    {
        private readonly IFinancialAssetHistoryRepository _financialAssetHistoryRepository;

        private readonly ICryptoHoldingRepository _cryptoHoldingRepository;
        private readonly IRepository<CryptoHoldingHistory> _cryptoHoldingHistoryRepository;
        private readonly IRepository<CryptoCurrency> _cryptoCurrencyRepository;

        public FinancialAssetRepository(ApplicationDbContext db,
            IFinancialAssetHistoryRepository financialAssetHistoryRepository,
            ICryptoHoldingRepository cryptoHoldingRepository,
            IRepository<CryptoHoldingHistory> cryptoHoldingHistoryRepository,
            ICryptoInfoService cryptoInfoService,
            IRepository<CryptoCurrency> cryptoCurrencyRepository) : base(db)
        {
            _financialAssetHistoryRepository = financialAssetHistoryRepository;
            _cryptoHoldingRepository = cryptoHoldingRepository;
            _cryptoHoldingHistoryRepository = cryptoHoldingHistoryRepository;
            _cryptoCurrencyRepository = cryptoCurrencyRepository;
        }

        public async Task UpdateAsync(FinancialAsset financialAsset, PortfolioData portfolioData)
        {
            using(var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var actualDateTime = DateTime.UtcNow;

                    financialAsset.LastUpdatedAt = actualDateTime;
                    financialAsset.Value = portfolioData.Value;

                    db.Update(financialAsset);

                    var cryptoCurrency = await _cryptoCurrencyRepository.GetAsync(
                        x => x.CurrencySymbol == portfolioData.CryptoCurrencySymbol);

                    var cryptoHolding = await _cryptoHoldingRepository
                        .GetAsync(x => x.FinancialAssetID == financialAsset.FinancialEntityId);

                    if (cryptoHolding is not null && portfolioData.Quantity is not null)
                    {
                        var quantity = (decimal)portfolioData.Quantity;

                        await _cryptoHoldingRepository.UpdateAsync(cryptoHolding, quantity);
                    }

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

                    await _financialAssetHistoryRepository.CreateAsync(financialAssetHistory);

                    var CryptoHoldingHistory = new CryptoHoldingHistory()
                    {
                        FinancialAssetHistoryID = financialAssetHistory.FinancialEntityId,
                        CryptoHoldingId = cryptoHolding!.CryptoHoldingEntityId,
                        CryptoCurrencId = cryptoCurrency!.CurrencyID,
                        Quantity = (decimal)portfolioData.Quantity,
                    };

                    await _cryptoHoldingHistoryRepository.CreateAsync(CryptoHoldingHistory);

                    await SaveAsync();

                    await transaction.CommitAsync();
                }
                catch (Exception)
                {

                    throw;
                }

            }
        }

        public async Task CreateAsync(FinancialAsset financialAsset, PortfolioData portfolioData)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {

                    var cryptoCurrencyName = string.Empty;

                    await base.CreateAsync(financialAsset);

                    var cryptoCurrency = await _cryptoCurrencyRepository.GetAsync(
                        x => x.CurrencySymbol == portfolioData.CryptoCurrencySymbol);

                    var CryptoHolding = new CryptoHolding() {
                        FinancialAssetID = financialAsset.FinancialEntityId,
                        CryptoCurrencId = cryptoCurrency!.CurrencyID,
                        Quantity = (decimal)portfolioData.Quantity!,
                    };

                    await _cryptoHoldingRepository.CreateAsync(CryptoHolding);


                    var financialAssetHistory = new FinancialAssetHistory()
                    {
                        UserID = financialAsset.UserID,
                        AssetPlatformID = financialAsset.AssetPlatformID,
                        AssetSourceID = financialAsset.AssetSourceID,
                        Value = financialAsset.Value,
                        FiatCurrencyID = financialAsset.FiatCurrencyID,
                        FinancialAssetId = financialAsset.FinancialEntityId,
                        RecordedAt = financialAsset.FirstRetrievedAt,
                    };

                    await _financialAssetHistoryRepository.CreateAsync(financialAssetHistory);

                    var CryptoHoldingHistory = new CryptoHoldingHistory()
                    {
                        FinancialAssetHistoryID = financialAssetHistory.FinancialEntityId,
                        CryptoHoldingId = CryptoHolding.CryptoHoldingEntityId,
                        CryptoCurrencId = cryptoCurrency!.CurrencyID,
                        Quantity = (decimal)portfolioData.Quantity,
                    };

                    await _cryptoHoldingHistoryRepository.CreateAsync(CryptoHoldingHistory);

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
                    .ThenInclude(x => x!.CryptoCurrency)
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
                    .ThenInclude(x => x!.CryptoCurrency)
                .Include(x => x.FiatCurrency)
                .LoadAsync();
                
           return await query.ToListAsync();

        }
    }
}
