using System.Text.Json.Serialization;

namespace OmniFi_API.DTOs.Kraken
{

    // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
    public record AssetsInfoResponse(
        [property: JsonPropertyName("error")] IReadOnlyList<object> error,
        [property: JsonPropertyName("result")] IReadOnlyDictionary<string,AssetInfo> result
    );

    public record AssetInfo(
        [property: JsonPropertyName("aclass")] string aclass,
        [property: JsonPropertyName("altname")] string altname,
        [property: JsonPropertyName("decimals")] int decimals,
        [property: JsonPropertyName("display_decimals")] int display_decimals,
        [property: JsonPropertyName("collateral_value")] string collateral_value,
        [property: JsonPropertyName("status")] string status
    );


}
