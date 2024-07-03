namespace OmniFi_API.Models.Api.CryptoDotCom
{
    public class CryptoDotComRequest
    {
        public required int Id { get; set; }
        public required string Method { get; set; }
        public Dictionary<string, string> Params { get; set; } = new Dictionary<string, string>();
        public string? Api_key { get; set; }
        public string? Sig { get; set; }
        public required long Nonce { get; set; }
    }
}
