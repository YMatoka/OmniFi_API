using OmniFi_API.Data;
using OmniFi_API.Models.Banks;
using OmniFi_API.Repository.Interfaces;

namespace OmniFi_API.Repository.Banks
{
    public class BankCredentialRepository : Repository<BankCredential>, IBankCredientalRepository
    {
        private readonly ApplicationDbContext _db;

        public BankCredentialRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task UpdateAsync(BankCredential bankCredential)
        {
            _db.Update(bankCredential);
            await _db.SaveChangesAsync();
        }
    }
}
