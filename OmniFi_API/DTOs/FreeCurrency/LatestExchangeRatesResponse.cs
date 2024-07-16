using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace OmniFi_API.DTOs.FreeCurrency
{
    public record LatestExchangeRatesResponse(
        [property: JsonPropertyName("data")] IReadOnlyDictionary<string, decimal> data
    );
}
