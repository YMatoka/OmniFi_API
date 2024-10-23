using OmniFi_API.Data;
using OmniFi_DTOs.Dtos.Banks;
using OmniFi_DTOs.Dtos.Cryptos;
using OmniFi_API.Models.Banks;
using OmniFi_API.Models.Cryptos;
using OmniFi_API.Models.Encryption;
using OmniFi_API.Repository.Cryptos;
using OmniFi_API.Repository.Interfaces;
using OmniFi_API.Services.Interfaces;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using OmniFi_API.Models.Api.Banks;
using System.Linq;

namespace OmniFi_API.Repository.Banks
{
    public class BankAccountRepository : BaseRepository<BankAccount>, IBankAccountRepository
    {

        public BankAccountRepository(
            ApplicationDbContext db) : base(db)
        {

        }

        public async Task<BankAccount?> GetWithEntitiesAsync(Expression<Func<BankAccount,bool>> filter, bool tracked = false)
        {
            IQueryable <BankAccount> query = _dbSet;

            if (!tracked)
            {
                query = query.AsNoTracking();
            }

            query = query.Where(filter);

            await query
                .Include(x => x.Bank)
                .Include(x => x.User)
                .Include(x => x.BankSubAccounts)
                .LoadAsync();


            return await query.FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(BankAccount bankAccount, BankRequisition bankRequisition)
        {

            bankAccount.RequisitionId = bankRequisition.Id;
            bankAccount.RequisitionCreatedAt = DateTime.UtcNow;

            await UpdateAsync(bankAccount);

        }

        public async Task UpdateAsync(BankAccount bankAccount)
        {
            db.Update(bankAccount);
            await SaveAsync();
        }

        public async Task UpdateAsync(BankAccount bankAccount, bool isAccessGranted)
        {
            bankAccount.IsAccessGranted = isAccessGranted;

            if(isAccessGranted)
                bankAccount.AccessGrantedAt = DateTime.UtcNow;

            await UpdateAsync(bankAccount);
        }

        public async Task UpdateAsync(BankAccount bankAccount, string requisitionId)
        {
            bankAccount.RequisitionId = requisitionId;
            await UpdateAsync(bankAccount);
        }
    }
}
