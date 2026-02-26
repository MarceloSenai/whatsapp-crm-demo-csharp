using Microsoft.EntityFrameworkCore;
using WhatsAppCrm.Web.Data;

namespace WhatsAppCrm.Web.Api;

public static class TemplatesApi
{
    public static IEndpointRouteBuilder MapTemplatesApi(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/templates", async (AppDbContext db) =>
        {
            var templates = await db.Templates
                .Where(t => t.Status == "approved")
                .OrderBy(t => t.Name)
                .ToListAsync();

            return Results.Ok(templates);
        });

        return app;
    }
}
