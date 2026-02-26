using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WhatsAppCrm.Web.Data;
using WhatsAppCrm.Web.Entities;
using WhatsAppCrm.Web.Services;

namespace WhatsAppCrm.Web.Api;

public static class CampaignsApi
{
    private record CreateCampaignRequest(string Name, string TemplateText, string? AudienceFilter, int? RateLimit);

    public static IEndpointRouteBuilder MapCampaignsApi(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/campaigns", async (AppDbContext db) =>
        {
            var campaigns = await db.Campaigns
                .Include(c => c.Messages)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            var result = campaigns.Select(c =>
            {
                var messages = c.Messages;
                return new
                {
                    c.Id,
                    c.Name,
                    c.TemplateText,
                    c.Status,
                    c.AudienceFilter,
                    c.RateLimit,
                    c.ScheduledAt,
                    c.StartedAt,
                    c.CompletedAt,
                    c.CreatedAt,
                    Stats = new
                    {
                        Total = messages.Count,
                        Pending = messages.Count(m => m.Status == "pending"),
                        Sent = messages.Count(m => m.Status == "sent"),
                        Delivered = messages.Count(m => m.Status == "delivered"),
                        Read = messages.Count(m => m.Status == "read"),
                        Replied = messages.Count(m => m.Status == "replied"),
                        Failed = messages.Count(m => m.Status == "failed")
                    }
                };
            });

            return Results.Ok(result);
        });

        app.MapPost("/api/campaigns", async (CreateCampaignRequest request, AppDbContext db) =>
        {
            if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.TemplateText))
                return Results.BadRequest(new { error = "name and templateText required" });

            // Parse audience filter for tag filtering
            var tagFilter = new List<string>();
            if (!string.IsNullOrEmpty(request.AudienceFilter))
            {
                try
                {
                    using var doc = JsonDocument.Parse(request.AudienceFilter);
                    if (doc.RootElement.TryGetProperty("tags", out var tagsElement))
                    {
                        foreach (var tag in tagsElement.EnumerateArray())
                        {
                            var val = tag.GetString();
                            if (val is not null) tagFilter.Add(val);
                        }
                    }
                }
                catch { /* ignore parse errors */ }
            }

            // Find eligible contacts (not opted out)
            var contacts = await db.Contacts
                .Where(c => !c.OptedOut)
                .ToListAsync();

            // Filter by tags if specified
            IEnumerable<Contact> eligible = contacts;
            if (tagFilter.Count > 0)
            {
                eligible = contacts.Where(c =>
                {
                    try
                    {
                        var contactTags = JsonSerializer.Deserialize<string[]>(c.Tags) ?? [];
                        return tagFilter.Any(t => contactTags.Contains(t));
                    }
                    catch { return false; }
                });
            }

            var eligibleList = eligible.ToList();

            var campaign = new Campaign
            {
                Name = request.Name,
                TemplateText = request.TemplateText,
                AudienceFilter = request.AudienceFilter,
                RateLimit = request.RateLimit ?? 30,
                Status = "draft"
            };
            db.Campaigns.Add(campaign);
            await db.SaveChangesAsync();

            foreach (var contact in eligibleList)
            {
                db.CampaignMessages.Add(new CampaignMessage
                {
                    CampaignId = campaign.Id,
                    ContactId = contact.Id,
                    Status = "pending"
                });
            }
            await db.SaveChangesAsync();

            return Results.Ok(new
            {
                campaign.Id,
                campaign.Name,
                campaign.TemplateText,
                campaign.Status,
                campaign.AudienceFilter,
                campaign.RateLimit,
                campaign.CreatedAt,
                EligibleContacts = eligibleList.Count
            });
        });

        app.MapPost("/api/campaigns/{id}/start", async (string id, AppDbContext db, CampaignRunner runner) =>
        {
            var campaign = await db.Campaigns.FindAsync(id);
            if (campaign is null)
                return Results.NotFound(new { error = "Campaign not found" });

            campaign.Status = "running";
            campaign.StartedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();

            // Enqueue for background processing
            runner.Enqueue(id);

            return Results.Ok(new { ok = true });
        });

        return app;
    }
}
