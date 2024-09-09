using OmniFi_API.Data;
using OmniFi_API.Models.Api.Banks;
using OmniFi_API.Repository.Interfaces;
using System.Linq.Expressions;

namespace OmniFi_API.Repository.Api.Banks
{
    public class BankDataApiCredentialRepository : BaseRepository<BankDataApiCredential>, IBankDataApiRepository
    {
        public BankDataApiCredentialRepository(ApplicationDbContext db) : base(db)
        {

        }

        public async Task UpdateAsync(BankDataApiCredential credential)
        {
            db.Update(credential);
            await db.SaveChangesAsync();
        }
    }
}
