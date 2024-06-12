using OmniFi_API.Models.Cryptos;

namespace OmniFi_API.Repository.Interfaces
{
    public interface ICryptoApiCredential : IRepository<CryptoApiCredential>
    {
        Task UpdateAsync(CryptoApiCredential cryptoApiCredential);
    }
}
