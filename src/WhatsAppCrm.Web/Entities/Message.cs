namespace WhatsAppCrm.Web.Entities;

public class Message
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..25];
    public string ConversationId { get; set; } = string.Empty;
    public Conversation Conversation { get; set; } = null!;
    public string Direction { get; set; } = "outbound";
    public string Type { get; set; } = "text";
    public string Content { get; set; } = string.Empty;
    public string Status { get; set; } = "sent";
    public string? Metadata { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
