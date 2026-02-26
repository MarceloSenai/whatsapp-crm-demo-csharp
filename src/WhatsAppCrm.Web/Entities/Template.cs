namespace WhatsAppCrm.Web.Entities;

public class Template
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..25];
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Status { get; set; } = "approved";
    public string? Variables { get; set; }
}
