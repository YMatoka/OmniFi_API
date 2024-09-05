using Newtonsoft.Json;

namespace OmniFi_API.DTOs.GoCardless
{
    public record BalancesResponse(
        [property: JsonProperty("balances")] IReadOnlyList<Balance> balances
    );
    public record Balance(
        [property: JsonProperty("balanceAmount")] BalanceAmount balanceAmount,
        [property: JsonProperty("balanceType")] string balanceType,
        [property: JsonProperty("referenceDate")] string referenceDate
    );

    public record BalanceAmount(
        [property: JsonProperty("amount")] string amount,
        [property: JsonProperty("currency")] string currency
    );



}
