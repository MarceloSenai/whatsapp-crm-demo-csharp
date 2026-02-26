using Microsoft.EntityFrameworkCore;
using WhatsAppCrm.Web.Data;

namespace WhatsAppCrm.Web.Api;

public static class PipelineApi
{
    private record MoveDealRequest(string DealId, string StageId);

    public static IEndpointRouteBuilder MapPipelineApi(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/pipeline", async (AppDbContext db) =>
        {
            var pipeline = await db.Pipelines
                .AsNoTracking()
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    Stages = p.Stages
                        .OrderBy(s => s.Order)
                        .Select(s => new
                        {
                            s.Id,
                            s.Name,
                            s.Order,
                            s.Color,
                            Deals = s.Deals
                                .OrderByDescending(d => d.UpdatedAt)
                                .Select(d => new
                                {
                                    d.Id,
                                    d.Title,
                                    d.Value,
                                    d.ContactId,
                                    Contact = new
                                    {
                                        d.Contact.Name,
                                        d.Contact.Tags
                                    }
                                }).ToList()
                        }).ToList()
                })
                .FirstOrDefaultAsync();

            return Results.Ok(pipeline);
        });

        app.MapPatch("/api/pipeline", async (MoveDealRequest request, AppDbContext db) =>
        {
            if (string.IsNullOrEmpty(request.DealId) || string.IsNullOrEmpty(request.StageId))
                return Results.BadRequest(new { error = "dealId and stageId required" });

            var deal = await db.Deals.FindAsync(request.DealId);
            if (deal is null)
                return Results.NotFound(new { error = "Deal not found" });

            deal.StageId = request.StageId;
            deal.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();

            return Results.Ok(new { ok = true });
        });

        return app;
    }
}
