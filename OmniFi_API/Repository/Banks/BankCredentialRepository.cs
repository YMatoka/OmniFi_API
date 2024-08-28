using OmniFi_API.Data;
using OmniFi_API.Models.Banks;
using OmniFi_API.Repository.Interfaces;

namespace OmniFi_API.Repository.Banks
{
    public class BankCredentialRepository : BaseRepository<BankAccount>, IBankCredentialRepository
    {


        public BankCredentialRepository(ApplicationDbContext db) : base(db)
        {
  
        }

        public async Task UpdateAsync(BankAccount bankCredential)
        {
            db.Update(bankCredential);
            await SaveAsync();
        }
    }
}
