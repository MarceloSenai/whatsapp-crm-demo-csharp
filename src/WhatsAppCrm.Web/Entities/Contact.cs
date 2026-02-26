namespace WhatsAppCrm.Web.Entities;

public class Contact
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..25];
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? AvatarUrl { get; set; }
    public string Tags { get; set; } = "[]";
    public bool OptedOut { get; set; }
    public DateTime? OptOutAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Conversation> Conversations { get; set; } = [];
    public ICollection<Deal> Deals { get; set; } = [];
    public ICollection<CampaignMessage> CampaignMessages { get; set; } = [];
}
