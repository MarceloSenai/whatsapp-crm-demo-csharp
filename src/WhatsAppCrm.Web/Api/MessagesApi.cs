using Microsoft.EntityFrameworkCore;
using WhatsAppCrm.Web.Data;
using WhatsAppCrm.Web.Entities;
using WhatsAppCrm.Web.Services;

namespace WhatsAppCrm.Web.Api;

public static class MessagesApi
{
    private record SendMessageRequest(string ConversationId, string Content);

    public static IEndpointRouteBuilder MapMessagesApi(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/messages", async (string conversationId, AppDbContext db) =>
        {
            if (string.IsNullOrEmpty(conversationId))
                return Results.BadRequest(new { error = "conversationId required" });

            var messages = await db.Messages
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();

            // Mark conversation as read
            await db.Conversations
                .Where(c => c.Id == conversationId)
                .ExecuteUpdateAsync(c => c.SetProperty(x => x.UnreadCount, 0));

            return Results.Ok(messages);
        });

        app.MapPost("/api/messages", async (SendMessageRequest request, AppDbContext db, IServiceScopeFactory scopeFactory) =>
        {
            if (string.IsNullOrEmpty(request.ConversationId) || string.IsNullOrEmpty(request.Content))
                return Results.BadRequest(new { error = "conversationId and content required" });

            // Save outbound message
            var outbound = new Message
            {
                ConversationId = request.ConversationId,
                Direction = "outbound",
                Content = request.Content,
                Status = "sent"
            };
            db.Messages.Add(outbound);
            await db.SaveChangesAsync();

            // Update conversation
            await db.Conversations
                .Where(c => c.Id == request.ConversationId)
                .ExecuteUpdateAsync(c => c
                    .SetProperty(x => x.LastMessageAt, DateTime.UtcNow)
                    .SetProperty(x => x.Status, "open"));

            // Capture values for the background task
            var outboundId = outbound.Id;
            var conversationId = request.ConversationId;
            var content = request.Content;

            // Fire-and-forget: simulate status progression and auto-response
            _ = Task.Run(async () =>
            {
                try
                {
                    using var scope = scopeFactory.CreateScope();
                    var bgDb = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    // Delivered after 1s
                    await Task.Delay(1000);
                    await bgDb.Messages
                        .Where(m => m.Id == outboundId)
                        .ExecuteUpdateAsync(m => m.SetProperty(x => x.Status, "delivered"));

                    // Read after 2s
                    await Task.Delay(2000);
                    await bgDb.Messages
                        .Where(m => m.Id == outboundId)
                        .ExecuteUpdateAsync(m => m.SetProperty(x => x.Status, "read"));

                    // Auto-response after random delay
                    var delay = WhatsAppSimulator.RandomDelayMs();
                    await Task.Delay(delay);

                    var responseText = WhatsAppSimulator.GenerateResponse(content);
                    bgDb.Messages.Add(new Message
                    {
                        ConversationId = conversationId,
                        Direction = "inbound",
                        Content = responseText,
                        Status = "read"
                    });
                    await bgDb.SaveChangesAsync();

                    // Increment unread count and update last message time
                    await bgDb.Conversations
                        .Where(c => c.Id == conversationId)
                        .ExecuteUpdateAsync(c => c
                            .SetProperty(x => x.LastMessageAt, DateTime.UtcNow)
                            .SetProperty(x => x.UnreadCount, x => x.UnreadCount + 1));
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Background message simulation error: {ex.Message}");
                }
            });

            return Results.Ok(outbound);
        });

        return app;
    }
}
