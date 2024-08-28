namespace OmniFi_API.Options.Banks
{
    public class BankInfoServiceOptions
    {
        public static string SectionName => nameof(BankInfoServiceOptions);
        public string ApiKey { get; set; } = string.Empty;
        public string ApiSecret { get; set; } = string.Empty;
    }
}
