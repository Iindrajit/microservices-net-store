namespace Basket.API.Models
{
    public class OutboxMessage
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = default!; // Fully qualified event type name
        public string Content { get; set; } = default!; // Serialized event content
        public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedOn { get; set; } // Null if not processed yet
    }
}
