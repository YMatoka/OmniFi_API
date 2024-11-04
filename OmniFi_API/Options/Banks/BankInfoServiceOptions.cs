using OmniFi_API.Utilities;

namespace OmniFi_API.Options.Banks
{
    public class BankInfoServiceOptions
    {
        public static string SectionName => nameof(BankInfoServiceOptions);

        public string ApiKey { get; set; } = string.Empty;
        public string ApiSecret => Environment.GetEnvironmentVariable(
            EnvironnementVariablesNames.BANK_INFO_SERVICE_SECRET) ?? string.Empty;

        public string ApiBaseUrl { get; set; } = string.Empty;
    }
}
