namespace WhatsAppCrm.Web.Entities;

public class Pipeline
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..25];
    public string Name { get; set; } = string.Empty;

    public ICollection<Stage> Stages { get; set; } = [];
}
