using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OmniFi_API.DTOs.Kraken
{
    public record UserBalanceResponse(
        [property: JsonPropertyName("error")] IReadOnlyList<object> error,
        [property: JsonPropertyName("result")] IReadOnlyDictionary<string, decimal> result
    );
}
