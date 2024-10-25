using OmniFi_DTOs.Dtos.Cryptos;
using OmniFi_API.Models.Cryptos;
using System.Linq.Expressions;
using OmniFi_API.Models.Banks;

namespace OmniFi_API.Repository.Interfaces
{
    public interface ICryptoExchangeAccountRepository : IRepository<CryptoExchangeAccount>
    {
        //public new Task RemoveAsync(CryptoExchangeAccount cryptoExchangeAccount);
        public Task CreateAsync(CryptoExchangeAccount cryptoExchangeAccount, CryptoExchangeAccountCreateDTO cryptoExchangeAccountCreateDTO);

        public Task<CryptoExchangeAccount?> GetWithEntitiesAsync(Expression<Func<CryptoExchangeAccount, bool>> filter, bool tracked = false);
    }
}
