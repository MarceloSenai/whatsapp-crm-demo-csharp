namespace WhatsAppCrm.Web.Entities;

public class Conversation
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..25];
    public string ContactId { get; set; } = string.Empty;
    public Contact Contact { get; set; } = null!;
    public string Status { get; set; } = "open";
    public string? AssignedTo { get; set; }
    public string Channel { get; set; } = "whatsapp";
    public DateTime? LastMessageAt { get; set; }
    public int UnreadCount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Message> Messages { get; set; } = [];
}
