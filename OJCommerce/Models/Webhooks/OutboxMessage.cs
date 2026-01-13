namespace OJCommerce.Models.Webhooks
{
    public class OutboxMessage
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public string Payload { get; set; }
        public bool Processed { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
