using System.Text.Json.Serialization;

namespace OmniFi_API.DTOs.GoCardless
{
    public record AccessTokenResponse(
        [property: JsonPropertyName("access")] string access,
        [property: JsonPropertyName("access_expires")] int access_expires,
        [property: JsonPropertyName("refresh")] string refresh,
        [property: JsonPropertyName("refresh_expires")] int refresh_expires
    );
}
