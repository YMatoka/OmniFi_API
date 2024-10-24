using Microsoft.Extensions.DependencyInjection;
using OmniFi_API.Factory.Interfaces;
using OmniFi_API.Services.Api.Banks;
using OmniFi_API.Services.Api.Cryptos;
using OmniFi_API.Services.Interfaces;
using OmniFi_API.Utilities;

namespace OmniFi_API.Factory
{
    public class FinancialAssetServiceFactory : IFinancialAssetServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public FinancialAssetServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IFinancialAssetService GetFinancialAssetRetriever(string platformName)
        {
            return platformName switch
            {
                CryptoExchangeNames.Binance =>
                    _serviceProvider.GetRequiredService<BinanceService>(),
                CryptoExchangeNames.Kraken => 
                    _serviceProvider.GetRequiredService<KrakenService>(),
                CryptoExchangeNames.CryptoDotCom =>
                    _serviceProvider.GetRequiredService<CryptoDotComService>(),
                BankNames.BoursoBank =>
                    _serviceProvider.GetRequiredService<BankInfoService>(),
                BankNames.CaisseDepargneIleDeFrance =>
                    _serviceProvider.GetRequiredService<BankInfoService>(),
                _ => throw new ArgumentException($"'{platformName}' is not a supported platform")
            };
        }
    }
}
