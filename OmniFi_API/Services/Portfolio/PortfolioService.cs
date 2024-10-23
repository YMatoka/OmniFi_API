using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using OmniFi_API.DTOs.CoinMarketCap;
using OmniFi_API.DTOs.FreeCurrency;
using OmniFi_API.Factory.Interfaces;
using OmniFi_API.Models.Assets;
using OmniFi_API.Models.Banks;
using OmniFi_API.Models.Cryptos;
using OmniFi_API.Models.Currencies;
using OmniFi_API.Models.Identity;
using OmniFi_API.Models.Portfolio;
using OmniFi_API.Options.Banks;
using OmniFi_API.Repository.Identity;
using OmniFi_API.Repository.Interfaces;
using OmniFi_API.Services.Api.Cryptos;
using OmniFi_API.Services.Interfaces;
using OmniFi_API.Utilities;
using System.Diagnostics.Metrics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace OmniFi_API.Services.Portfolio
{
    public class PortfolioService : IFetchPortfolioService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICryptoExchangeAccountRepository _cryptoExchangeAccountRepository;
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly IFinancialAssetHistoryRepository _financialAssetHistoryRepository;
        private readonly IFinancialAssetRepository _financialAssetRepository;
        private readonly ICryptoApiCredentialRepository _cryptoApiCredentialRepository;

        private readonly IRepository<AssetPlatform> _assetPlatformRepository;
        private readonly IRepository<Bank> _bankRepository;
        private readonly IRepository<CryptoExchange> _cryptoExchangeRepository;
        private readonly IRepository<FiatCurrency> _fiatCurrencyRepository;
        private readonly IRepository<AssetSource> _assetSourceRepository;
        private readonly IRepository<CryptoCurrency> _cryptoCurrencyRepository;
        private readonly IRepository<BankSubAccount> _bankSubAccountRepository;

        private readonly IStringEncryptionService _stringEncryptionService;
        private readonly IFiatCurrencyService _fiatCurrencyService;
        private readonly ICryptoInfoService _cryptoInfoService;
        private readonly IBankInfoService _bankInfoService;

        private readonly IFinancialAssetServiceFactory _financialAssetServiceFactory;
        private readonly BankInfoServiceOptions _bankInfoServiceOptions;

        private Dictionary<string, decimal> _ConversionRates;
        private IEnumerable<CryptoInfo>? _cryptoInfos;

        private readonly IEqualityComparer<BankSubAccount> _bankSubAccountEqualityComparer;

        public PortfolioService(
            IUserRepository userRepository,
            IFinancialAssetHistoryRepository financialAssetHistoryRepository,
            IFinancialAssetRepository financialAssetRepository,
            ICryptoExchangeAccountRepository cryptoExchangeAccountRepository,
            IBankAccountRepository bankAccountRepository,
            IStringEncryptionService stringEncryptionService,
            IRepository<FiatCurrency> fiatCurrencyRepository,
            IRepository<AssetPlatform> assetPlatformRepository,
            IRepository<Bank> bankRepository,
            IRepository<CryptoExchange> cryptoExchangeRepository,
            IRepository<AssetSource> assetSourceRepository,
            ICryptoApiCredentialRepository cryptoApiCredentialRepository,
            IFiatCurrencyService fiatCurrencyService,
            ICryptoInfoService cryptoInfoService,
            IRepository<CryptoCurrency> cryptoCurrencyRepository,
            IFinancialAssetServiceFactory financialAssetServiceFactory,
            IOptions<BankInfoServiceOptions> options,
            IBankInfoService bankInfoService)
        {
            _userRepository = userRepository;
            _financialAssetHistoryRepository = financialAssetHistoryRepository;
            _financialAssetRepository = financialAssetRepository;
            _cryptoExchangeAccountRepository = cryptoExchangeAccountRepository;
            _bankAccountRepository = bankAccountRepository;
            _stringEncryptionService = stringEncryptionService;
            _fiatCurrencyRepository = fiatCurrencyRepository;
            _assetPlatformRepository = assetPlatformRepository;
            _bankRepository = bankRepository;
            _cryptoExchangeRepository = cryptoExchangeRepository;
            _assetSourceRepository = assetSourceRepository;
            _cryptoApiCredentialRepository = cryptoApiCredentialRepository;
            _fiatCurrencyService = fiatCurrencyService;
            _cryptoInfoService = cryptoInfoService;

            _cryptoCurrencyRepository = cryptoCurrencyRepository;
            _ConversionRates = new();
            _financialAssetServiceFactory = financialAssetServiceFactory;
            _bankInfoServiceOptions = options.Value;
            _bankInfoService = bankInfoService;
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
                {
                    foreach (var exchangeAccount in user.CryptoExchangeAccounts)
                    {
            
                        var account = await _cryptoExchangeAccountRepository.GetWithEntitiesAsync(
                            x => x.UserID == user.Id && x.CryptoExchangeID == exchangeAccount.CryptoExchangeID,
                            tracked: true);

                        if (account is not null)
                            cryptoExchangeAccounts.Add(account);
                    }
                }
                   

                if (user.BankAccounts is not null)
                {
                    foreach (var bankAccount in user.BankAccounts)
                    {

                        var account = await _bankAccountRepository.GetWithEntitiesAsync(
                            x => x.UserID == user.Id && x.BankId == bankAccount.BankId, tracked : true);

                        if (account is not null)
                            bankAccounts.Add(account);
                    }
                }
            }

            foreach (var exchangeAccount in cryptoExchangeAccounts)
            {
                await FetchCryptoPortfolio(exchangeAccount, user);
            }

            foreach (var bankAccount in bankAccounts)
            {
                await CheckBankAccess(bankAccount);

                if(bankAccount.IsAccessGranted && bankAccount.BankSubAccounts is not null)
                    await FetchBankPortfolio(bankAccount, user);
            }
        }

        private async Task CheckBankAccess(BankAccount bankAccount)
        {
            if (bankAccount.IsAccessGranted)
            {
                if(DateTime.Compare(
                        bankAccount.AccessGrantedAt.AddDays(bankAccount.AccessDurationInDays),
                        DateTime.UtcNow) < 0)
                {
                    await _bankAccountRepository.UpdateAsync(bankAccount, isAccessGranted: false);
                }
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

        private async Task<IEnumerable<PortfolioData>?> GetBankPortfolioDataAsync(
            BankAccount bankAccount)
        {

            List<PortfolioData> portfolioDatas = new List<PortfolioData>();

            if(bankAccount.Bank is null)
                return null;

            if (bankAccount.BankSubAccounts is null)
                return null;
        
            var financialAssetRetriever = _financialAssetServiceFactory
                        .GetFinancialAssetRetriever(bankAccount.Bank.BankName);

            foreach (var subBankAccount in bankAccount.BankSubAccounts)
            {

                var datas = await financialAssetRetriever.GetUserBalanceAsync(
                    _bankInfoServiceOptions.ApiKey,
                    _bankInfoServiceOptions.ApiSecret,
                    subBankAccount.BankSubAccountID,
                    bankAccount.Bank.BankName);

                if (datas is not null)
                    portfolioDatas.AddRange(datas);
            }
            
            return portfolioDatas;
        }


        private async Task FetchCryptoPortfolio(CryptoExchangeAccount cryptoExchangeAccount, ApplicationUser user)
        {
            var portFolioDatas = await GetCryptoPortfolioDataAsync(cryptoExchangeAccount);


            if (portFolioDatas is not null)
            {

                await UpdateCryptoCurrencies(portFolioDatas);

                await UpdateFiatCurrencies(portFolioDatas);

                foreach (var portfolioData in portFolioDatas)
                {
                    await UpdatePortfolioAsync(user, portfolioData);
                }
            }
        }

        private async Task UpdateFiatCurrencies(IEnumerable<PortfolioData> portFolioDatas)
        {
            IEnumerable<string> fiatSymbolList = GetFiatSymbolList(portFolioDatas);

            IEnumerable<string> nonExistingFiatSymbolList = await GetNonExistingFiatSymbolList(fiatSymbolList);

            if (nonExistingFiatSymbolList.Count() > 0)
            {
                var currencyInfos = await _fiatCurrencyService.GetCurrenciesInfo(nonExistingFiatSymbolList);

                if (currencyInfos is not null)
                {
                    await AddFiatCurrencies(currencyInfos);
                }
            }
        }

        private async Task UpdateCryptoCurrencies(IEnumerable<PortfolioData> portFolioDatas)
        {
            var cryptoSymbolList = GetCryptoSymbolList(portFolioDatas);

            var nonExistingCryptoSymbolList = await GetNonExistingCrypoSymbolList(cryptoSymbolList);

            if (nonExistingCryptoSymbolList.Count() > 0)
            {
                var cryptoInfos = await _cryptoInfoService.GetAllCryptoInfos(nonExistingCryptoSymbolList);

                if (cryptoInfos is not null)
                {
                    await AddCryptoCurrencies(cryptoInfos);
                }

            }
        }

        private async Task AddFiatCurrencies(IEnumerable<CurrencyInfo> currencyInfos)
        {
            foreach (var currencyIfo in currencyInfos)
            {
                await _fiatCurrencyRepository.CreateAsync(new FiatCurrency()
                {
                    CurrencyName = currencyIfo.name,
                    CurrencySymbol = currencyIfo.symbol,
                    CurrencyCode = currencyIfo.code,
                });
            }
        }

        private async Task<IEnumerable<string>> GetNonExistingFiatSymbolList(IEnumerable<string> fiatSymbolList)
        {
            List<string> result = new List<string>();

            foreach (var fiatSymbol in fiatSymbolList)
            {
                var cryptoCurrency = await _fiatCurrencyRepository
                    .GetAsync(x => x.CurrencyCode == fiatSymbol);

                if (cryptoCurrency is null)
                    result.Add(fiatSymbol);
            }

            return result;
        }

        private async Task AddCryptoCurrencies(IEnumerable<CryptoInfo> cryptoInfos)
        {
            foreach(var cryptoInfo in cryptoInfos) 
            {
                await _cryptoCurrencyRepository.CreateAsync(new CryptoCurrency()
                {
                    CurrencyName = cryptoInfo.name,
                    CurrencySymbol = cryptoInfo.symbol,
                });
            }
        }

        private async Task<IEnumerable<string>> GetNonExistingCrypoSymbolList(IEnumerable<string> cryptoSymbolList)
        {
            List<string> result = new List<string>();

            foreach(var cryptoSymbol in cryptoSymbolList)
            {
                var cryptoCurrency = await _cryptoCurrencyRepository
                    .GetAsync(x => x.CurrencySymbol == cryptoSymbol);

                if (cryptoCurrency is null)
                    result.Add(cryptoSymbol);
            }

            return result;
        }

        private static IEnumerable<string> GetCryptoSymbolList(IEnumerable<PortfolioData> portFolioDatas)
            => portFolioDatas
            .Where(x => x.CryptoCurrencySymbol is not null)
            .Select(x => x.CryptoCurrencySymbol ?? string.Empty)
            .Distinct();

        private static IEnumerable<string> GetFiatSymbolList(IEnumerable<PortfolioData> portFolioDatas)
            => portFolioDatas
            .Where(x => x.FiatCurrencyCode is not null)
            .Select(x => x.FiatCurrencyCode ?? string.Empty)
            .Distinct();

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
                    return await _financialAssetServiceFactory
                        .GetFinancialAssetRetriever(CryptoExchangeNames.CryptoDotCom)
                        .GetUserBalanceAsync(apiKey, apiSecret);;

                case CryptoExchangeNames.Binance:
                    return await _financialAssetServiceFactory
                        .GetFinancialAssetRetriever(CryptoExchangeNames.Binance)
                        .GetUserBalanceAsync(apiKey, apiSecret);

                case CryptoExchangeNames.Kraken:
                    return await _financialAssetServiceFactory
                        .GetFinancialAssetRetriever(CryptoExchangeNames.Kraken)
                        .GetUserBalanceAsync(apiKey, apiSecret);

                default:
                    return null;
            }
        }

        private async Task UpdatePortfolioAsync(ApplicationUser user, PortfolioData portfolioData)
        {
            var SelectedFiatCurrency = await _fiatCurrencyRepository.GetAsync(
                x => x.FiatCurrencyID == user.SelectedFiatCurrencyID);

            if (SelectedFiatCurrency!.CurrencyCode != portfolioData.FiatCurrencyCode)
            {

                if (!_ConversionRates.ContainsKey(portfolioData.FiatCurrencyCode))
                {
                    var conversionRate = await _fiatCurrencyService.GetConversionRate(
                        portfolioData.FiatCurrencyCode, SelectedFiatCurrency!.CurrencyCode);

                    if(conversionRate is not null)
                    {
                        _ConversionRates[portfolioData.FiatCurrencyCode] = (decimal)conversionRate;
                    }
                    
                }

                if (_ConversionRates.ContainsKey(portfolioData.FiatCurrencyCode))
                {
                    portfolioData.Balance = portfolioData.Balance * _ConversionRates[portfolioData.FiatCurrencyCode];
                }
                
            }

            var financialAsset = await _financialAssetRepository.GetWithEntitiesAsync(
                x => x.UserID == user.Id &&
                x.AssetSource!.AssetSourceName == portfolioData.AssetSourceName &&
                ((x.AssetPlatform!.Bank != null && x.AssetPlatform.Bank.BankName == portfolioData.AssetPlatformName) ||
                (x.AssetPlatform.CryptoExchange != null && x.AssetPlatform.CryptoExchange.ExchangeName == portfolioData.AssetPlatformName 
                && x.CryptoHolding!.CryptoCurrency!.CurrencySymbol == portfolioData.CryptoCurrencySymbol)),
                tracked:true
                );

            if( financialAsset is not null )
            {
                await _financialAssetRepository.UpdateAsync(financialAsset, portfolioData);       
            }
            else
            {
                AssetPlatform? assetPlatform = null;

                // ?? add fiat currency service to add currency does not exists in the database
                var fiatCurrency = _ConversionRates.ContainsKey(portfolioData.FiatCurrencyCode) ?
                    await _fiatCurrencyRepository.GetAsync(x => x.CurrencyCode == SelectedFiatCurrency.CurrencyCode) :
                    await _fiatCurrencyRepository.GetAsync(x => x.CurrencyCode == portfolioData.FiatCurrencyCode);


                switch (portfolioData.AssetSourceName)
                {
                    case AssetSourceNames.CryptoHolding:
                        var cryptoExchange = await _cryptoExchangeRepository.GetAsync(x => x.ExchangeName == portfolioData.AssetPlatformName);
                        if (cryptoExchange is not null)
                        {
                            assetPlatform = await _assetPlatformRepository.GetAsync(
                                x => x.CryptoExchangeID == cryptoExchange.CryptoExchangeID);
                        }
                        break;
                    default:
                        var bank = await _bankRepository.GetAsync(x => x.BankName == portfolioData.AssetPlatformName);
                        if (bank is not null)
                        {
                            assetPlatform = await _assetPlatformRepository.GetAsync(
                                x => x.BankID == bank.BankID);
                        }
                        break;
                }

                var assetSource = await _assetSourceRepository.GetAsync(x => x.AssetSourceName == portfolioData.AssetSourceName);

                await _financialAssetRepository.CreateAsync(new FinancialAsset()
                {
                    UserID = user.Id,
                    AssetPlatformID = assetPlatform!.AssetPlatformID,
                    AssetSourceID = assetSource!.AssetSourceID,
                    FiatCurrencyID = fiatCurrency!.FiatCurrencyID,
                    FirstRetrievedAt = DateTime.UtcNow,
                    LastUpdatedAt = DateTime.UtcNow,
                    Amount = portfolioData.Balance
                }, 
                portfolioData); 
                
            }

        }


    }
}
