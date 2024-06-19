namespace OmniFi_API.Options.Cryptos
{
    public class CryptoApiCredentialOptions
    {
        public static string SectionName = nameof(CryptoApiCredentialOptions);
        public string ApiKeyEncryptionKey { get; set; } = string.Empty;
        public string ApiSecretEncryptionKey { get; set; } = string.Empty;
    }
}
