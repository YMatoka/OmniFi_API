using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OmniFi_API.DTOs.Kraken
{
    public record Balance(
        [property: JsonPropertyName("balance")] decimal balance,
        [property: JsonPropertyName("hold_trade")] decimal hold_trade
    );

    public record UserBalanceResponse(
        [property: JsonPropertyName("error")] IReadOnlyList<object> error,
        [property: JsonPropertyName("result")] IReadOnlyDictionary<string, Balance> result
    );
}
