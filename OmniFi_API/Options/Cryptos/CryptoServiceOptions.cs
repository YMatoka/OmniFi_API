namespace OmniFi_API.Options.Cryptos
{
    public class CryptoInfoServiceOptions
    {
        public static string SectionName = nameof(CryptoInfoServiceOptions);
        public string ApiBaseUrl { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string Separator {  get; set; } = string.Empty;

    }
}
