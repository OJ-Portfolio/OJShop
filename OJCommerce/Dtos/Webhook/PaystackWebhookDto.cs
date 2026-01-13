using System.Text.Json.Serialization;

namespace OJCommerce.Dtos.Webhook
{
    public class PaystackWebhookDto
    {
        [JsonPropertyName("event")]
        public string Event { get; set; }

        [JsonPropertyName("data")]
        public PaystackDataDto Data { get; set; }
    }

    public class PaystackDataDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("reference")]
        public string Reference { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("authorization")]
        public PaystackAuthorizationDto Authorization { get; set; }

        [JsonPropertyName("channel")]
        public string Channel { get; set; }

        [JsonPropertyName("customer")]
        public PaystackCustomerDto Customer { get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, string> Metadata { get; set; }
    }

    public class PaystackAuthorizationDto
    {
        [JsonPropertyName("authorization_code")]
        public string AuthorizationCode { get; set; }

        [JsonPropertyName("last4")]
        public string Last4 { get; set; }

        [JsonPropertyName("brand")]
        public string Brand { get; set; }

        [JsonPropertyName("reusable")]
        public bool Reusable { get; set; }
    }

    public class PaystackCustomerDto
    {
        [JsonPropertyName("customer_code")]
        public string CustomerCode { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }
    }
}