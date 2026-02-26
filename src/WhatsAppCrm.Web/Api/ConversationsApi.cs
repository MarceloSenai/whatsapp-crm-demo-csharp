using Microsoft.EntityFrameworkCore;
using WhatsAppCrm.Web.Data;

namespace WhatsAppCrm.Web.Api;

public static class ConversationsApi
{
    public static IEndpointRouteBuilder MapConversationsApi(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/conversations", async (AppDbContext db) =>
        {
            var conversations = await db.Conversations
                .Include(c => c.Contact)
                .Include(c => c.Messages.OrderByDescending(m => m.CreatedAt).Take(1))
                .OrderByDescending(c => c.LastMessageAt)
                .Select(c => new
                {
                    c.Id,
                    c.ContactId,
                    c.Status,
                    c.AssignedTo,
                    c.Channel,
                    c.LastMessageAt,
                    c.UnreadCount,
                    c.CreatedAt,
                    Contact = new
                    {
                        c.Contact.Id,
                        c.Contact.Name,
                        c.Contact.Phone,
                        c.Contact.AvatarUrl
                    },
                    LastMessage = c.Messages
                        .OrderByDescending(m => m.CreatedAt)
                        .Select(m => new
                        {
                            m.Content,
                            m.CreatedAt
                        })
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Results.Ok(conversations);
        });

        return app;
    }
}
