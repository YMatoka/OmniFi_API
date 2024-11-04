using OmniFi_API.Utilities;

namespace OmniFi_API.Options.Cryptos
{
    public class CryptoInfoServiceOptions
    {
        public static string SectionName = nameof(CryptoInfoServiceOptions);
        public string ApiKey => Environment.GetEnvironmentVariable(
            EnvironnementVariablesNames.CRYPTO_INFO_SERVICE_SECRET) ?? string.Empty;
        public string ApiBaseUrl { get; set; } = string.Empty;
        public string Separator {  get; set; } = string.Empty;

    }
}
