using OmniFi_API.Dtos.Banks;
using OmniFi_API.Models.Banks;

namespace OmniFi_API.Repository.Interfaces
{
    public interface IBankAccountRepository : IRepository<BankAccount>
    {
        public Task CreateAsync(BankAccount bankAccount, BankAccountCreateDTO bankAccountCreateDTO);
    }
}
