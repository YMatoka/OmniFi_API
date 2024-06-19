namespace OmniFi_API.Options.Encryption
{
    public class StringEncryptionServiceOptions
    {
        public static string SectionName => nameof(StringEncryptionServiceOptions);
        public string IV { get; set; } = string.Empty;
    }
}
