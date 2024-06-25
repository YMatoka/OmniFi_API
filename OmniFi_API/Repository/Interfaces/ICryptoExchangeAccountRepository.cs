using OmniFi_DTOs.Dtos.Cryptos;
using OmniFi_API.Models.Cryptos;

namespace OmniFi_API.Repository.Interfaces
{
    public interface ICryptoExchangeAccountRepository : IRepository<CryptoExchangeAccount>
    {
        public Task CreateAsync(CryptoExchangeAccount cryptoExchangeAccount, CryptoExchangeAccountCreateDTO cryptoExchangeAccountCreateDTO);
    }
}
