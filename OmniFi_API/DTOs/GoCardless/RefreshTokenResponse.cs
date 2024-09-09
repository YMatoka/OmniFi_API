using System.Text.Json.Serialization;

namespace OmniFi_API.DTOs.GoCardless
{
    public record RefreshTokenResponse
    (
        [property: JsonPropertyName("access")] string access,
        [property: JsonPropertyName("access_expires")] int access_expires
    );      
}
