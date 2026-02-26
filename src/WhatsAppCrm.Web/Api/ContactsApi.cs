using Microsoft.EntityFrameworkCore;
using WhatsAppCrm.Web.Data;

namespace WhatsAppCrm.Web.Api;

public static class ContactsApi
{
    private record ToggleOptOutRequest(string Id, bool OptedOut);

    public static IEndpointRouteBuilder MapContactsApi(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/contacts", async (AppDbContext db) =>
        {
            var contacts = await db.Contacts
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Phone,
                    c.Email,
                    c.AvatarUrl,
                    c.Tags,
                    c.OptedOut,
                    c.OptOutAt,
                    c.CreatedAt,
                    c.UpdatedAt,
                    ConversationCount = c.Conversations.Count,
                    DealCount = c.Deals.Count
                })
                .ToListAsync();

            return Results.Ok(contacts);
        });

        app.MapPatch("/api/contacts", async (ToggleOptOutRequest request, AppDbContext db) =>
        {
            if (string.IsNullOrEmpty(request.Id))
                return Results.BadRequest(new { error = "id required" });

            var contact = await db.Contacts.FindAsync(request.Id);
            if (contact is null)
                return Results.NotFound(new { error = "Contact not found" });

            contact.OptedOut = request.OptedOut;
            contact.OptOutAt = request.OptedOut ? DateTime.UtcNow : null;
            contact.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();

            return Results.Ok(contact);
        });

        return app;
    }
}
