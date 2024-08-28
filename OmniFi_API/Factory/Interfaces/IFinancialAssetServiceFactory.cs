using OmniFi_API.Services.Interfaces;

namespace OmniFi_API.Factory.Interfaces
{
    public interface IFinancialAssetServiceFactory
    {
        public IFinancialAssetService GetFinancialAssetRetriever(string platformName);
    }
}
