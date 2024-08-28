using System.Text.Json.Serialization;

namespace OmniFi_API.DTOs.Binance
{
    public record TickerInfoResponse(
        [property: JsonPropertyName("symbol")] string symbol,
        [property: JsonPropertyName("price")] string price
    );

}
