using OmniFi_API.Models.Cryptos;

namespace OmniFi_API.Repository.Interfaces
{
    public interface ICryptoHoldingRepository : IRepository<CryptoHolding>
    {
        public Task UpdateAsync(CryptoHolding cryptoHolding, decimal quantity);
    }
}
