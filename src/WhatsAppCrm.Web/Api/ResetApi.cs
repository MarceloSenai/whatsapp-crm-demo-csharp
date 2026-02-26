using WhatsAppCrm.Web.Data;

namespace WhatsAppCrm.Web.Api;

public static class ResetApi
{
    public static IEndpointRouteBuilder MapResetApi(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/reset", async (AppDbContext db) =>
        {
            try
            {
                // Delete all in FK order
                db.CampaignMessages.RemoveRange(db.CampaignMessages);
                await db.SaveChangesAsync();

                db.Campaigns.RemoveRange(db.Campaigns);
                await db.SaveChangesAsync();

                db.Templates.RemoveRange(db.Templates);
                await db.SaveChangesAsync();

                db.Messages.RemoveRange(db.Messages);
                await db.SaveChangesAsync();

                db.Conversations.RemoveRange(db.Conversations);
                await db.SaveChangesAsync();

                db.Deals.RemoveRange(db.Deals);
                await db.SaveChangesAsync();

                db.Stages.RemoveRange(db.Stages);
                await db.SaveChangesAsync();

                db.Pipelines.RemoveRange(db.Pipelines);
                await db.SaveChangesAsync();

                db.Contacts.RemoveRange(db.Contacts);
                await db.SaveChangesAsync();

                // Re-seed
                await DatabaseSeeder.SeedAsync(db);

                return Results.Ok(new { ok = true });
            }
            catch (Exception ex)
            {
                return Results.Json(
                    new { ok = false, error = ex.Message },
                    statusCode: 500);
            }
        });

        return app;
    }
}
