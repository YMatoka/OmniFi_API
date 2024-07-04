using OmniFi_API.Repository.Identity;
using OmniFi_API.Repository.Interfaces;
using OmniFi_API.Services.Interfaces;

namespace OmniFi_API.Services.Portfolio
{
    public class PortfolioService : IFetchPortfolioService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICryptoApiCredentialRepository _cryptoApiCredentialRepository;
        private readonly IBankCredentialRepository _bankCredentialRepository;
        private readonly IFinancialAssetHistoryRepository _financialAssetHistoryRepository;
        private readonly IFinancialAssetRepository _financialAssetRepository;

        public PortfolioService(
            IUserRepository userRepository, 
            ICryptoApiCredentialRepository cryptoApiCredentialRepository, 
            IBankCredentialRepository bankCredentialRepository, 
            IFinancialAssetHistoryRepository financialAssetHistoryRepository, 
            IFinancialAssetRepository financialAssetRepository)
        {
            _userRepository = userRepository;
            _cryptoApiCredentialRepository = cryptoApiCredentialRepository;
            _bankCredentialRepository = bankCredentialRepository;
            _financialAssetHistoryRepository = financialAssetHistoryRepository;
            _financialAssetRepository = financialAssetRepository;
        }

        public Task FetchPortfolio(string userName, string? bankName = null, string? cryptoExchangeName = null)
        {
            throw new NotImplementedException();
        }
    }
}
