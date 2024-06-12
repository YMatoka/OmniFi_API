using OmniFi_API.Data;
using OmniFi_API.Models.Cryptos;
using OmniFi_API.Repository.Interfaces;
using System.Linq.Expressions;

namespace OmniFi_API.Repository.Cryptos
{
    public class CryptoApiCredentialRepository : Repository<CryptoApiCredential>, ICryptoApiCredential
    {
        private readonly ApplicationDbContext _db;

        public CryptoApiCredentialRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task UpdateAsync(CryptoApiCredential cryptoApiCredential)
        {
            _db.Update(cryptoApiCredential);
            await _db.SaveChangesAsync();
        }
    }
}
