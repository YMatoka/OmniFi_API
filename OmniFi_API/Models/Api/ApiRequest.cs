using static OmniFi_API.Utilities.ApiTypes;

namespace OmniFi_API.Models.Api
{
    public class ApiRequest
    {
        public required ApiType ApiType { get; set; }
        public required string Url { get; set; } 
        public object? Data { get; set; }
    }
}
