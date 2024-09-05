using System.Text.Json.Serialization;

namespace OmniFi_API.DTOs.GoCardless
{
    public record AgreementResponse(
        [property: JsonPropertyName("id")] string id,
        [property: JsonPropertyName("created")] DateTime created,
        [property: JsonPropertyName("max_historical_days")] int max_historical_days,
        [property: JsonPropertyName("access_valid_for_days")] int access_valid_for_days,
        [property: JsonPropertyName("access_scope")] IReadOnlyList<string> access_scope,
        [property: JsonPropertyName("accepted")] string accepted,
        [property: JsonPropertyName("institution_id")] string institution_id
        );
}
