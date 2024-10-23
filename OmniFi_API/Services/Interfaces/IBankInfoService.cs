
using OmniFi_API.Models.Api.Banks;

namespace OmniFi_API.Services.Interfaces
{
    public interface IBankInfoService
    {
        Task<BankRequisition?> GetRequisition(string apiKey, string apiSecret, string institutionId, string? redirectUrl = null);
        Task<IEnumerable<string>?> GetSubAccounts(string apiKey, string apiSecret, string requisitionId);
    }
}
