namespace OmniFi_API.Models
{
    public class ReferentialAPIEndpoints
    {
        public int ReferentialAPIEndpointsID { get; set; }
        public required string APIEndpoint { get; set; }
        public required string ReferentialType { get; set; }
    }
}
