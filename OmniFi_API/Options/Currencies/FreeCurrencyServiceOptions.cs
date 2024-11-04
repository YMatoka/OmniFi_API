using OmniFi_API.Utilities;

namespace OmniFi_API.Options.Currencies
{
    public class FiatCurrencyServiceOptions
    {
        public static string SectionName = nameof(FiatCurrencyServiceOptions);
        public string ApiKey => Environment.GetEnvironmentVariable(
            EnvironnementVariablesNames.FIAT_CURRENCY_SERVICE_SECRET) ?? string.Empty;
        public string ApiBaseUrl { get; set; } = string.Empty;
        public string Separator { get; set; } = string.Empty;
    }
}
