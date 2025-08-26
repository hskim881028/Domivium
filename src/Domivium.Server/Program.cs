using Domivium.Server.Data;
using Domivium.Server.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Services
builder.Services.AddGrpc();
builder.Services
    .AddPostgresql(builder.Configuration)
    .AddRedis(builder.Configuration)
    .AddJwtAuth(builder.Configuration)
    .AddMagicOnionWithFilters()
    .AddAppServicesScoped()
    .AddAppServicesSingleton();

// Build
var app = builder.Build();

// For Test DB
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// Middleware
app.UseAuthentication();
app.UseAuthorization();

// Endpoints
app.MapMagicOnionService();
app.MapGet("/", () => "Domivium gRPC Server is running");

app.Run();