using System.Text.Json.Serialization;

namespace OmniFi_API.DTOs.GoCardless
{

     public record WebhookResponse(
        [property: JsonPropertyName("events")] IReadOnlyList<Event> events
    );

    // WebhookResponse myDeserializedClass = JsonSerializer.Deserialize<WebhookResponse>(myJsonResponse);
    public record Event(
        [property: JsonPropertyName("id")] string id,
        [property: JsonPropertyName("created_at")] DateTime created_at,
        [property: JsonPropertyName("action")] string action,
        [property: JsonPropertyName("resource_type")] string resource_type,
        [property: JsonPropertyName("links")] Links links,
        [property: JsonPropertyName("details")] Details details
    );
    public record Details(
            [property: JsonPropertyName("origin")] string origin,
            [property: JsonPropertyName("cause")] string cause,
            [property: JsonPropertyName("description")] string description,
            [property: JsonPropertyName("scheme")] string scheme,
            [property: JsonPropertyName("reason_code")] string reason_code
        );



        public record Links(
            [property: JsonPropertyName("mandate")] string mandate,
            [property: JsonPropertyName("organisation")] string organisation
        );



    
}
