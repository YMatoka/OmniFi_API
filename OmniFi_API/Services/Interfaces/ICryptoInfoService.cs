using OmniFi_API.DTOs.CoinMarketCap;

namespace OmniFi_API.Services.Interfaces
{
    public interface ICryptoInfoService : IBaseService
    {
        Task<IEnumerable<CryptoInfo>?> GetAllCryptoInfos(IEnumerable<string> cryptoSymbolList);
    }
}