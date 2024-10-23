using OmniFi_DTOs.Dtos.Banks;
using OmniFi_API.Models.Banks;
using System.Linq.Expressions;
using OmniFi_API.Models.Api.Banks;

namespace OmniFi_API.Repository.Interfaces
{
    public interface IBankAccountRepository : IRepository<BankAccount>
    {
        public Task<BankAccount?> GetWithEntitiesAsync(Expression<Func<BankAccount, bool>> filter, bool tracked = false);
        public Task UpdateAsync(BankAccount bankAccount);
        public Task UpdateAsync(BankAccount bankAccount, BankRequisition bankRequisition);
        public Task UpdateAsync(BankAccount bankAccount, bool isAccessGranted);
        public Task UpdateAsync(BankAccount bankAccount, string requisitionId);
    }
}
