using System.Text.Json.Serialization;

namespace OmniFi_API.DTOs.GoCardless
{
    public record RequisitionResponse(
        [property: JsonPropertyName("id")] string id,
        [property: JsonPropertyName("created")] DateTime created,
        [property: JsonPropertyName("redirect")] string redirect,
        [property: JsonPropertyName("status")] string status,
        [property: JsonPropertyName("institution_id")] string institution_id,
        [property: JsonPropertyName("agreement")] string agreement,
        [property: JsonPropertyName("reference")] string reference,
        [property: JsonPropertyName("accounts")] IReadOnlyList<string> accounts,
        [property: JsonPropertyName("user_language")] string user_language,
        [property: JsonPropertyName("link")] string link,
        [property: JsonPropertyName("ssn")] object ssn,
        [property: JsonPropertyName("account_selection")] bool account_selection,
        [property: JsonPropertyName("redirect_immediate")] bool redirect_immediate
    );
}
