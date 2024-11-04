using OmniFi_API.Utilities;

namespace OmniFi_API.Options.Identity
{
    public class JwtSettingsOptions
    {
        public static string GetSectionName => nameof(JwtSettingsOptions);

        public string Key =>
            Environment.GetEnvironmentVariable(EnvironnementVariablesNames.JWT_SECRET) ?? string.Empty;
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        public required double ExpirationTimeInDays { get; set; }

    }
}
