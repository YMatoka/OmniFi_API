namespace OmniFi_API.Models.Api.CryptoDotCom
{
    public class CryptoDotComRequest
    {
        public required int id { get; set; }
        public required string method { get; set; }
        public Dictionary<string, string> @params { get; set; } = new Dictionary<string, string>();
        public string? api_key { get; set; }
        public string? sig { get; set; }
        public required string nonce { get; set; }
    }
}
