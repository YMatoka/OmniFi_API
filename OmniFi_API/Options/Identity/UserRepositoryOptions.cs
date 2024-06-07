namespace OmniFi_API.Options.Identity
{
    public class UserRepositoryOptions
    {
        public static string SectionName = nameof(UserRepositoryOptions);

        public string SecretKey { get; set; } = string.Empty;

        public int ExpirationTime { get ; set; } = default;
    }
}
