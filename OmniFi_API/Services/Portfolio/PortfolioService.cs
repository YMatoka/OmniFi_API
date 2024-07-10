using OmniFi_API.Models.Assets;
using OmniFi_API.Models.Banks;
using OmniFi_API.Models.Cryptos;
using OmniFi_API.Models.Currencies;
using OmniFi_API.Models.Identity;
using OmniFi_API.Models.Portfolio;
using OmniFi_API.Repository.Identity;
using OmniFi_API.Repository.Interfaces;
using OmniFi_API.Services.Interfaces;
using OmniFi_API.Utilities;
using System.Runtime.CompilerServices;

namespace OmniFi_API.Services.Portfolio
{
    public class PortfolioService : IFetchPortfolioService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICryptoExchangeAccountRepository _cryptoExchangeAccountRepository;
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly IFinancialAssetHistoryRepository _financialAssetHistoryRepository;
        private readonly IFinancialAssetRepository _financialAssetRepository;
         
        private readonly IRepository<AssetPlatform> _assetPlatformRepository;
        private readonly IRepository<Bank> _bankRepository;
        private readonly IRepository<CryptoExchange> _cryptoExchangeRepository;
        private readonly IRepository<FiatCurrency> _fiatCurrencyRepository;
        private readonly IRepository<AssetSource> _assetSourceRepository;

        private readonly ICryptoDotComService _cryptoDotComService;
        private readonly IStringEncryptionService _stringEncryptionService;

        public PortfolioService(
            IUserRepository userRepository,
            IFinancialAssetHistoryRepository financialAssetHistoryRepository,
            IFinancialAssetRepository financialAssetRepository,
            ICryptoExchangeAccountRepository cryptoExchangeAccountRepository,
            IBankAccountRepository bankAccountRepository,
            ICryptoDotComService cryptoDotComService,
            IStringEncryptionService stringEncryptionService,
            IRepository<FiatCurrency> fiatCurrencyRepository,
            IRepository<AssetPlatform> assetPlatformRepository,
            IRepository<Bank> bankRepository,
            IRepository<CryptoExchange> cryptoExchangeRepository,
            IRepository<AssetSource> assetSourceRepository)
        {
            _userRepository = userRepository;
            _financialAssetHistoryRepository = financialAssetHistoryRepository;
            _financialAssetRepository = financialAssetRepository;
            _cryptoExchangeAccountRepository = cryptoExchangeAccountRepository;
            _bankAccountRepository = bankAccountRepository;
            _cryptoDotComService = cryptoDotComService;
            _stringEncryptionService = stringEncryptionService;
            _fiatCurrencyRepository = fiatCurrencyRepository;
            _assetPlatformRepository = assetPlatformRepository;
            _bankRepository = bankRepository;
            _cryptoExchangeRepository = cryptoExchangeRepository;
            _assetSourceRepository = assetSourceRepository;
        }

        public async Task FetchPortfolio(string userName, string? bankName = null, string? cryptoExchangeName = null)
        {
            List<BankAccount> bankAccounts = new();
            List<CryptoExchangeAccount> cryptoExchangeAccounts = new();

            var user = await _userRepository.GetWithAllAccountsAsync(userName);

            if (user is null)
                return;

            if(bankName is not null)
            {
                var bankAccount = await _bankAccountRepository.GetWithEntitiesAsync(
                    x => x.UserID == user.Id && x.Bank!.BankName == bankName);

                if (bankAccount is not null)
                    bankAccounts.Add(bankAccount);
            }

            if (cryptoExchangeName is not null)
            {
                var cryptoExchangeAccount = await _cryptoExchangeAccountRepository.GetWithEntitiesAsync(
                    x => x.UserID == user.Id && x.CryptoExchange!.ExchangeName == cryptoExchangeName);

                if (cryptoExchangeAccount is not null)
                    cryptoExchangeAccounts.Add(cryptoExchangeAccount);
            }

            if(bankName is null && cryptoExchangeName is null) 
            {
                if(user.CryptoExchangeAccounts is not null)
                    cryptoExchangeAccounts.AddRange(user.CryptoExchangeAccounts);

                if (user.BankAccounts is not null)
                    bankAccounts.AddRange(user.BankAccounts);
            }

            foreach (var exchangeAccount in cryptoExchangeAccounts)
            {
                await FetchCryptoPortfolio(exchangeAccount, user);
            }

            foreach (var account in bankAccounts)
            {
                await FetchBankPortfolio(account, user);
            }
        }

        private async Task FetchBankPortfolio(BankAccount bankAccount, ApplicationUser user)
        {
            var portFolioDatas = await GetBankPortfolioDataAsync(bankAccount);

            if (portFolioDatas is not null)
            {
                foreach (var portfolioData in portFolioDatas)
                {
                    await UpdatePortfolioAsync(user, portfolioData);
                }
            }
        }

        private async Task<IEnumerable<PortfolioData>?> GetBankPortfolioDataAsync(BankAccount bankAccount)
        {
            throw new NotImplementedException();
        }


        private async Task FetchCryptoPortfolio(CryptoExchangeAccount cryptoExchangeAccount, ApplicationUser user)
        {
            var portFolioDatas = await GetCryptoPortfolioDataAsync(cryptoExchangeAccount);

            if (portFolioDatas is not null)
            {
                foreach (var portfolioData in portFolioDatas)
                {
                    await UpdatePortfolioAsync(user, portfolioData);
                }
            }
        }

        private async Task<IEnumerable<PortfolioData>?> GetCryptoPortfolioDataAsync(CryptoExchangeAccount cryptoExchangeAccount)
        {
            var apiKey = await _stringEncryptionService.DecryptAsync(
                cryptoExchangeAccount.CryptoApiCredential!.ApiKey,
                cryptoExchangeAccount.CryptoApiCredential.AesKey!.Key,
                cryptoExchangeAccount.CryptoApiCredential.AesIV!.IV);

            var apiSecret = await _stringEncryptionService.DecryptAsync(
                cryptoExchangeAccount.CryptoApiCredential!.ApiSecret,
                cryptoExchangeAccount.CryptoApiCredential.AesKey!.Key,
                cryptoExchangeAccount.CryptoApiCredential.AesIV!.IV);

            switch (cryptoExchangeAccount.CryptoExchange!.ExchangeName)
            {
                case CryptoExchangeNames.CryptoDotCom:
                    return await _cryptoDotComService.GetUserBalanceAsync(apiKey, apiSecret);

                case CryptoExchangeNames.Binance:

                case CryptoExchangeNames.Kraken:

                default:
                    return null;
            }
        }

        private async Task UpdatePortfolioAsync(ApplicationUser user, PortfolioData portfolioData)
        {
            var financialAsset = await _financialAssetRepository.GetWithEntitiesAsync(
                x => x.UserID == user.Id &&
                x.AssetSource!.AssetSourceName == portfolioData.AssetSourceName &&
                ((x.AssetPlatform!.Bank != null && x.AssetPlatform.Bank.BankName == portfolioData.AssetPlatformName) ||
                (x.AssetPlatform.CryptoExchange != null && x.AssetPlatform.CryptoExchange.ExchangeName == portfolioData.AssetPlatformName 
                && x.CryptoHolding!.CryptoCurrencySymbol == portfolioData.CryptoCurrencySymbol))
                );

            if( financialAsset is not null )
            {
                await _financialAssetRepository.UpdateAsync(financialAsset, portfolioData.Value);       
            }
            else
            {
                AssetPlatform? assetPlatform = null;

                var fiatCurrency = await _fiatCurrencyRepository.GetAsync(
                    x => x.CurrencyCode == portfolioData.FiatCurrencyCode);

                var bank = await _bankRepository.GetAsync(x => x.BankName == portfolioData.AssetPlatformName);
                var cryptoExchange = await _cryptoExchangeRepository.GetAsync(x => x.ExchangeName == portfolioData.AssetPlatformName);
                var assetSource = await _assetSourceRepository.GetAsync(x => x.AssetSourceName == portfolioData.AssetSourceName);

                if (bank is not null)
                {
                    assetPlatform = await _assetPlatformRepository.GetAsync(
                        x => x.BankID == bank.BankID);
                }

                if (cryptoExchange is not null)
                {
                    assetPlatform = await _assetPlatformRepository.GetAsync(
                        x => x.CryptoExchangeID == cryptoExchange.CryptoExchangeID);
                }

                await _financialAssetRepository.CreateAsync(new FinancialAsset()
                {
                    UserID = user.Id,
                    AssetPlatformID = assetPlatform!.AssetPlatformID,
                    AssetSourceID = assetSource!.AssetSourceID,
                    FiatCurrencyID = fiatCurrency!.FiatCurrencyID,
                    FirstRetrievedAt = DateTime.UtcNow,
                    LastUpdatedAt = DateTime.UtcNow,
                    Value = portfolioData.Value
                }); 
                
            }

        }


    }
}
