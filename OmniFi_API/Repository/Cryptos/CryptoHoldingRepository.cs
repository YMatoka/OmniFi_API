using OmniFi_API.Data;
using OmniFi_API.Models.Cryptos;
using OmniFi_API.Repository.Interfaces;
using System.Linq.Expressions;

namespace OmniFi_API.Repository.Cryptos
{
    public class CryptoHoldingRepository : BaseRepository<CryptoHolding>, ICryptoHoldingRepository
    {
        public CryptoHoldingRepository(ApplicationDbContext db) : base(db)
        {
            
        }

        public async Task UpdateAsync(CryptoHolding cryptoHolding, decimal quantity)
        {
            cryptoHolding.Quantity = quantity;
            db.Update(cryptoHolding);
            await SaveAsync();
        }
    }
}
