namespace WhatsAppCrm.Web.Entities;

public class Deal
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..25];
    public string ContactId { get; set; } = string.Empty;
    public Contact Contact { get; set; } = null!;
    public string StageId { get; set; } = string.Empty;
    public Stage Stage { get; set; } = null!;
    public double Value { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
