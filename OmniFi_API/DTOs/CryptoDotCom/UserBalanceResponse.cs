using System.Text.Json.Serialization;

namespace OmniFi_API.DTOs.CryptoDotCom
{

    // Root myDeserializedClass = JsonSerializer.Deserialize<UserBalanceResponse>(myJsonResponse);
    public record UserBalanceResponse(
        [property: JsonPropertyName("id")] int id,
        [property: JsonPropertyName("method")] string method,
        [property: JsonPropertyName("code")] int code,
        [property: JsonPropertyName("result")] Result result
    );

    public record Result(
        [property: JsonPropertyName("data")] IReadOnlyList<Datum> data
    );

    public record Datum(
        [property: JsonPropertyName("total_available_balance")] string total_available_balance,
        [property: JsonPropertyName("total_margin_balance")] string total_margin_balance,
        [property: JsonPropertyName("total_initial_margin")] string total_initial_margin,
        [property: JsonPropertyName("total_position_im")] string total_position_im,
        [property: JsonPropertyName("total_haircut")] string total_haircut,
        [property: JsonPropertyName("total_maintenance_margin")] string total_maintenance_margin,
        [property: JsonPropertyName("total_position_cost")] string total_position_cost,
        [property: JsonPropertyName("total_cash_balance")] string total_cash_balance,
        [property: JsonPropertyName("total_collateral_value")] string total_collateral_value,
        [property: JsonPropertyName("total_session_unrealized_pnl")] string total_session_unrealized_pnl,
        [property: JsonPropertyName("instrument_name")] string instrument_name,
        [property: JsonPropertyName("total_session_realized_pnl")] string total_session_realized_pnl,
        [property: JsonPropertyName("is_liquidating")] bool is_liquidating,
        [property: JsonPropertyName("total_effective_leverage")] string total_effective_leverage,
        [property: JsonPropertyName("position_limit")] string position_limit,
        [property: JsonPropertyName("used_position_limit")] string used_position_limit,
        [property: JsonPropertyName("position_balances")] IReadOnlyList<PositionBalance> position_balances
    );

    public record PositionBalance(
        [property: JsonPropertyName("instrument_name")] string instrument_name,
        [property: JsonPropertyName("quantity")] string quantity,
        [property: JsonPropertyName("market_value")] string market_value,
        [property: JsonPropertyName("collateral_eligible")] string collateral_eligible,
        [property: JsonPropertyName("haircut")] string haircut,
        [property: JsonPropertyName("collateral_amount")] string collateral_amount,
        [property: JsonPropertyName("max_withdrawal_balance")] string max_withdrawal_balance,
        [property: JsonPropertyName("reserved_qty")] string reserved_qty
    );



}


