namespace WhatsAppCrm.Web.Entities;

public class Stage
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..25];
    public string PipelineId { get; set; } = string.Empty;
    public Pipeline Pipeline { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
    public string Color { get; set; } = "#6366f1";

    public ICollection<Deal> Deals { get; set; } = [];
}
