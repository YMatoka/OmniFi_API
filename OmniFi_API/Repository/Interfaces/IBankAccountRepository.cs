using OmniFi_DTOs.Dtos.Banks;
using OmniFi_API.Models.Banks;
using System.Linq.Expressions;

namespace OmniFi_API.Repository.Interfaces
{
    public interface IBankAccountRepository : IRepository<BankSubAccount>
    {
        public Task CreateAsync(BankSubAccount bankAccount, BankAccountCreateDTO bankAccountCreateDTO);
        Task<BankSubAccount?> GetWithEntitiesAsync(Expression<Func<BankSubAccount,bool>> filter, bool tracked = false);
    }
}
