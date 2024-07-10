using OmniFi_DTOs.Dtos.Cryptos;
using OmniFi_API.Models.Cryptos;
using System.Linq.Expressions;

namespace OmniFi_API.Repository.Interfaces
{
    public interface ICryptoExchangeAccountRepository : IRepository<CryptoExchangeAccount>
    {
        public Task CreateAsync(CryptoExchangeAccount cryptoExchangeAccount, CryptoExchangeAccountCreateDTO cryptoExchangeAccountCreateDTO);
        Task<CryptoExchangeAccount?> GetWithEntitiesAsync(Expression<Func<CryptoExchangeAccount, bool>> filter, bool tracked = false);
    }
}
