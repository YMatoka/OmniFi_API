using OmniFi_API.Models.Banks;
using OmniFi_API.Models.Cryptos;

namespace OmniFi_API.Repository.Interfaces
{
    public interface IBankCredentialRepository : IRepository<BankCredential>
    {
        Task UpdateAsync(BankCredential bankCredential);
    }
}
