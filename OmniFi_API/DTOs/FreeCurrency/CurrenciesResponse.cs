using Newtonsoft.Json;

namespace OmniFi_API.DTOs.FreeCurrency
{
    public record CurrenciesResponse(
        [property: JsonProperty("data")] IReadOnlyDictionary<string, CurrencyInfo> data
    );

    public record CurrencyInfo(
        [property: JsonProperty("symbol")] string symbol,
        [property: JsonProperty("name")] string name,
        [property: JsonProperty("symbol_native")] string symbol_native,
        [property: JsonProperty("decimal_digits")] int? decimal_digits,
        [property: JsonProperty("rounding")] int? rounding,
        [property: JsonProperty("code")] string code,
        [property: JsonProperty("name_plural")] string name_plural,
        [property: JsonProperty("type")] string type
    );
}
