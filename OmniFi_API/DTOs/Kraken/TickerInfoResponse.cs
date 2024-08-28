using System.Text.Json.Serialization;

namespace OmniFi_API.DTOs.Kraken
{
    // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);

    public record TickerInfoResponse(
        [property: JsonPropertyName("error")] IReadOnlyList<object> error,
        [property: JsonPropertyName("result")] IReadOnlyDictionary<string, TickerInfo> result
    );

    public record TickerInfo(
        [property: JsonPropertyName("a")] IReadOnlyList<string> a,
        [property: JsonPropertyName("b")] IReadOnlyList<string> b,
        [property: JsonPropertyName("c")] IReadOnlyList<string> c,
        [property: JsonPropertyName("v")] IReadOnlyList<string> v,
        [property: JsonPropertyName("p")] IReadOnlyList<string> p,
        [property: JsonPropertyName("t")] IReadOnlyList<int> t,
        [property: JsonPropertyName("l")] IReadOnlyList<string> l,
        [property: JsonPropertyName("h")] IReadOnlyList<string> h,
        [property: JsonPropertyName("o")] string o
    );

}
