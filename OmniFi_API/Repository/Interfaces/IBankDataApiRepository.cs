using OmniFi_API.Models.Api.Banks;

namespace OmniFi_API.Repository.Interfaces
{
    public interface IBankDataApiRepository : IRepository<BankDataApiCredential>
    {
       public Task UpdateAsync(BankDataApiCredential credential);

    }
}
