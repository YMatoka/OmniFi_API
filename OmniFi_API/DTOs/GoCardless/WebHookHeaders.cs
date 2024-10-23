using Microsoft.AspNetCore.Mvc;

namespace OmniFi_API.DTOs.GoCardless
{
    public class WebHookHeaders
    {
        [FromHeader(Name = "User-Agent")]
        public string? UserAgent { get; set; }

        [FromHeader(Name = "Content-Type")]
        public string? ContentType { get; set; }

        [FromHeader(Name = "Webhook-Signature")]
        public string? WebhookSignature { get; set; }
    }
}
