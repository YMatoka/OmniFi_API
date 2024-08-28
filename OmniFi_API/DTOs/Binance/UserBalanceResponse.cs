using System.Text.Json.Serialization;

namespace OmniFi_API.DTOs.Binance

{   
    // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
    public record UserBalanceResponse(
        [property: JsonPropertyName("makerCommission")] int makerCommission,
        [property: JsonPropertyName("takerCommission")] int takerCommission,
        [property: JsonPropertyName("buyerCommission")] int buyerCommission,
        [property: JsonPropertyName("sellerCommission")] int sellerCommission,
        [property: JsonPropertyName("commissionRates")] CommissionRates commissionRates,
        [property: JsonPropertyName("canTrade")] bool canTrade,
        [property: JsonPropertyName("canWithdraw")] bool canWithdraw,
        [property: JsonPropertyName("canDeposit")] bool canDeposit,
        [property: JsonPropertyName("brokered")] bool brokered,
        [property: JsonPropertyName("requireSelfTradePrevention")] bool requireSelfTradePrevention,
        [property: JsonPropertyName("preventSor")] bool preventSor,
        [property: JsonPropertyName("updateTime")] long updateTime,
        [property: JsonPropertyName("accountType")] string accountType,
        [property: JsonPropertyName("balances")] IReadOnlyList<Balance> balances,
        [property: JsonPropertyName("permissions")] IReadOnlyList<string> permissions,
        [property: JsonPropertyName("uid")] int uid
    );


    public record Balance(
        [property: JsonPropertyName("asset")] string asset,
        [property: JsonPropertyName("free")] string free,
        [property: JsonPropertyName("locked")] string locked
    );

    public record CommissionRates(
        [property: JsonPropertyName("maker")] string maker,
        [property: JsonPropertyName("taker")] string taker,
        [property: JsonPropertyName("buyer")] string buyer,
        [property: JsonPropertyName("seller")] string seller
    );


}
