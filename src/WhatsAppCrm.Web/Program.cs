using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WhatsAppCrm.Web.Api;
using WhatsAppCrm.Web.Components;
using WhatsAppCrm.Web.Data;
using WhatsAppCrm.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Database — supports DATABASE_URL env var (Render) or ConnectionStrings config
var connStr = Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (connStr != null && connStr.StartsWith("postgresql://"))
{
    var uri = new Uri(connStr);
    var userInfo = uri.UserInfo.Split(':');
    var host = uri.Host;
    var dbPort = uri.Port > 0 ? uri.Port : 5432;
    var database = uri.AbsolutePath.TrimStart('/');
    connStr = $"Host={host};Port={dbPort};Database={database};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connStr));

// CampaignRunner as singleton BackgroundService
builder.Services.AddSingleton<ICampaignQueue, CampaignRunner>();
builder.Services.AddHostedService(sp => (CampaignRunner)sp.GetRequiredService<ICampaignQueue>());

// HttpClient for Blazor components to call local API
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.Services.AddHttpClient("LocalApi", client =>
{
    client.BaseAddress = new Uri($"http://localhost:{port}");
});
builder.Services.AddScoped(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    return factory.CreateClient("LocalApi");
});

// Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// JSON serialization for Minimal APIs
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

// Kestrel port
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

var app = builder.Build();

// Auto-create tables and seed on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();

    if (!await db.Contacts.AnyAsync())
    {
        await DatabaseSeeder.SeedAsync(db);
    }
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseAntiforgery();
app.MapStaticAssets();

// Map Minimal API endpoints
app.MapConversationsApi();
app.MapMessagesApi();
app.MapPipelineApi();
app.MapCampaignsApi();
app.MapContactsApi();
app.MapTemplatesApi();
app.MapResetApi();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
