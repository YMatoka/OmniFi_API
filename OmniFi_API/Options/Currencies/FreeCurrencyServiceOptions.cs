namespace OmniFi_API.Options.Currencies
{
    public class FiatCurrencyServiceOptions
    {
        public static string SectionName = nameof(FiatCurrencyServiceOptions);
        public string ApiBaseUrl { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string Separator { get; set; } = string.Empty;
    }
}
