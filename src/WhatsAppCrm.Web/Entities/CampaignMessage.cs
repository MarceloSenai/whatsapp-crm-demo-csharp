namespace WhatsAppCrm.Web.Entities;

public class CampaignMessage
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..25];
    public string CampaignId { get; set; } = string.Empty;
    public Campaign Campaign { get; set; } = null!;
    public string ContactId { get; set; } = string.Empty;
    public Contact Contact { get; set; } = null!;
    public string Status { get; set; } = "pending";
    public DateTime? SentAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime? RepliedAt { get; set; }
}
