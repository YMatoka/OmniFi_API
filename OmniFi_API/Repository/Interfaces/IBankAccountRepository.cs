using OmniFi_DTOs.Dtos.Banks;
using OmniFi_API.Models.Banks;
using System.Linq.Expressions;

namespace OmniFi_API.Repository.Interfaces
{
    public interface IBankAccountRepository : IRepository<BankSubAccount>
    {
        public Task CreateAsync(BankAccount bankAccount, BankAccountCreateDTO bankAccountCreateDTO);
        Task<BankAccount?> GetWithEntitiesAsync(Expression<Func<BankAccount, bool>> filter, bool tracked = false);
    }
}
