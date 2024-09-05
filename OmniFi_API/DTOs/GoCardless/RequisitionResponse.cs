using System.Text.Json.Serialization;

namespace OmniFi_API.DTOs.GoCardless
{
    public record RequisitionResponse(
    [property: JsonPropertyName("id")] string id,
    [property: JsonPropertyName("status")] string status,
    [property: JsonPropertyName("agreements")] string agreements,
    [property: JsonPropertyName("accounts")] IReadOnlyList<string> accounts,
    [property: JsonPropertyName("reference")] string reference
    );
}
