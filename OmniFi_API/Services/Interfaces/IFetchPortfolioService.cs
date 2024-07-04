namespace OmniFi_API.Services.Interfaces
{
    public interface IFetchPortfolioService
    {
        public Task FetchPortfolio(string userName, string? bankName = null, string? cryptoExchangeName = null);
    }
}
