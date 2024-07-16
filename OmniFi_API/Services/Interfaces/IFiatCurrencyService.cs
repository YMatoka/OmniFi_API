using OmniFi_API.DTOs.FreeCurrency;

namespace OmniFi_API.Services.Interfaces
{
    public interface IFiatCurrencyService
    {
        Task<decimal?> GetConversionRate(string inputCurrency, string targetCurrency);
        Task<IEnumerable<CurrencyInfo>?> GetCurrenciesInfo(IEnumerable<string> currencyList);
    }
}