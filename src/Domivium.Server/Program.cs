using Domivium.Server.Data;
using Domivium.Server.UseCases.Login;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    // 개발 환경에서는 SSL 없이도 동작. 운영은 SSL Mode=Require 권장
    var connectionString = builder.Configuration.GetConnectionString("Default");
    opt.UseNpgsql(connectionString);
});

builder.Services.AddMagicOnion().AddJsonTranscoding();

// // JwtBearer authentication
// builder.Services.AddAuthentication().AddJwtBearer();
// builder.Services.AddAuthorization();

builder.Services.AddScoped<ILoginUseCase, LoginUseCase>();

var app = builder.Build();

// For develop
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.MapMagicOnionService();
app.MapGet("/", () => "Domivium gRPC Server is running");
app.Run();