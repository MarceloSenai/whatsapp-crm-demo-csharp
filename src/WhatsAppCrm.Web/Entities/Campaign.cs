namespace WhatsAppCrm.Web.Entities;

public class Campaign
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..25];
    public string Name { get; set; } = string.Empty;
    public string TemplateText { get; set; } = string.Empty;
    public string Status { get; set; } = "draft";
    public string? AudienceFilter { get; set; }
    public int RateLimit { get; set; } = 30;
    public DateTime? ScheduledAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<CampaignMessage> Messages { get; set; } = [];
}
