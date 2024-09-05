using System.Text.Json.Serialization;

namespace OmniFi_API.DTOs.GoCardless
{
    public record LinkResponse(
        [property: JsonPropertyName("id")] string id,
        [property: JsonPropertyName("redirect")] string redirect,
        [property: JsonPropertyName("status")] IReadOnlyDictionary<string,string> status,
        [property: JsonPropertyName("agreement")] string agreement,
        [property: JsonPropertyName("accounts")] IReadOnlyList<string> accounts,
        [property: JsonPropertyName("reference")] string reference,
        [property: JsonPropertyName("user_language")] string user_language,
        [property: JsonPropertyName("link")] string link
        );
}
