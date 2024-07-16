using Microsoft.CodeAnalysis;
using System.Text.Json.Serialization;

namespace OmniFi_API.DTOs.CoinMarketCap
{
    // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);

    public record CryptoInfoResponse(
        [property: JsonPropertyName("data")] IReadOnlyDictionary<string, CryptoInfo[]> data,
        [property: JsonPropertyName("status")] Status status
    );

    public record CryptoInfo(
        [property: JsonPropertyName("id")] int id,
        [property: JsonPropertyName("name")] string name,
        [property: JsonPropertyName("symbol")] string symbol,
        [property: JsonPropertyName("category")] string category,
        [property: JsonPropertyName("description")] string description,
        [property: JsonPropertyName("slug")] string slug,
        [property: JsonPropertyName("logo")] string logo,
        [property: JsonPropertyName("subreddit")] string subreddit,
        [property: JsonPropertyName("notice")] string notice,
        [property: JsonPropertyName("tags")] IReadOnlyList<string> tags,
        [property: JsonPropertyName("tag-names")] IReadOnlyList<string> tagnames,
        [property: JsonPropertyName("tag-groups")] IReadOnlyList<string> taggroups,
        [property: JsonPropertyName("urls")] Urls urls,
        [property: JsonPropertyName("platform")] Platform platform,
        [property: JsonPropertyName("date_added")] DateTime date_added,
        [property: JsonPropertyName("twitter_username")] string twitter_username,
        [property: JsonPropertyName("is_hidden")] int is_hidden,
        [property: JsonPropertyName("date_launched")] object date_launched,
        [property: JsonPropertyName("contract_address")] IReadOnlyList<ContractAddress> contract_address,
        [property: JsonPropertyName("self_reported_circulating_supply")] object self_reported_circulating_supply,
        [property: JsonPropertyName("self_reported_tags")] object self_reported_tags,
        [property: JsonPropertyName("self_reported_market_cap")] object self_reported_market_cap,
        [property: JsonPropertyName("infinite_supply")] bool infinite_supply
    );

    public record ContractAddress(
        [property: JsonPropertyName("contract_address")] string contract_address,
        [property: JsonPropertyName("platform")] Platform platform
    );


    public record Platform(
        [property: JsonPropertyName("id")] string id,
        [property: JsonPropertyName("name")] string name,
        [property: JsonPropertyName("slug")] string slug,
        [property: JsonPropertyName("symbol")] string symbol,
        [property: JsonPropertyName("token_address")] string token_address,
        [property: JsonPropertyName("coin")] Coin coin
    );


    public record Coin(
        [property: JsonPropertyName("id")] string id,
        [property: JsonPropertyName("name")] string name,
        [property: JsonPropertyName("symbol")] string symbol,
        [property: JsonPropertyName("slug")] string slug
    );

    public record Status(
        [property: JsonPropertyName("timestamp")] DateTime timestamp,
        [property: JsonPropertyName("error_code")] int error_code,
        [property: JsonPropertyName("error_message")] string error_message,
        [property: JsonPropertyName("elapsed")] int elapsed,
        [property: JsonPropertyName("credit_count")] int credit_count,
        [property: JsonPropertyName("notice")] string notice
    );

    public record Urls(
        [property: JsonPropertyName("website")] IReadOnlyList<string> website,
        [property: JsonPropertyName("technical_doc")] IReadOnlyList<string> technical_doc,
        [property: JsonPropertyName("twitter")] IReadOnlyList<object> twitter,
        [property: JsonPropertyName("reddit")] IReadOnlyList<string> reddit,
        [property: JsonPropertyName("message_board")] IReadOnlyList<string> message_board,
        [property: JsonPropertyName("announcement")] IReadOnlyList<object> announcement,
        [property: JsonPropertyName("chat")] IReadOnlyList<object> chat,
        [property: JsonPropertyName("explorer")] IReadOnlyList<string> explorer,
        [property: JsonPropertyName("source_code")] IReadOnlyList<string> source_code
    );


}
