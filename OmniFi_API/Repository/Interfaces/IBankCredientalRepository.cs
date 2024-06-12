using OmniFi_API.Models.Banks;
using OmniFi_API.Models.Cryptos;

namespace OmniFi_API.Repository.Interfaces
{
    public interface IBankCredientalRepository : IRepository<BankCredential>
    {
        Task UpdateAsync(BankCredential bankCredential);
    }
}
