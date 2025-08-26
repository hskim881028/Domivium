using System.Text;
using Domivium.Server.AppServices;
using Domivium.Server.Data;
using Domivium.Server.Filters;
using Domivium.Server.Settings;
using MagicOnion.Server;
using MagicOnion.Server.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

namespace Domivium.Server.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPostgresql(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>((sp, opt) =>
        {
            var cs = config.GetConnectionString("Default") ?? throw new InvalidOperationException("ConnectionStrings:Default is missing.");
            opt.UseNpgsql(cs);

            foreach (var interceptor in sp.GetServices<SaveChangesInterceptor>())
            {
                opt.AddInterceptors(interceptor);
            }
        });
        return services;
    }

    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var cs = config.GetConnectionString("Redis") ?? throw new InvalidOperationException("ConnectionStrings:Redis is missing.");
            return ConnectionMultiplexer.Connect(cs);
        });

        services.AddSingleton<RedLockFactory>(sp =>
        {
            var mux = sp.GetRequiredService<IConnectionMultiplexer>();
            return RedLockFactory.Create([new RedLockMultiplexer(mux)]);
        });
        return services;
    }

    public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<JwtSettings>(config.GetSection("Jwt"));
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<JwtSettings>>().Value);

        var jwtSettings = config.GetSection("Jwt").Get<JwtSettings>();
        if (jwtSettings == null)
        {
            throw new InvalidOperationException("JwtSettings is missing.");
        }

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ClockSkew = TimeSpan.FromSeconds(jwtSettings.ClockSkewSeconds)
        };
        services.AddSingleton(validationParameters);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => options.TokenValidationParameters = validationParameters);

        services.AddAuthorization();
        return services;
    }

    public static IServiceCollection AddMagicOnionWithFilters(this IServiceCollection services)
    {
        services.AddMagicOnion().AddJsonTranscoding();

        services.Configure<MagicOnionOptions>(opt =>
        {
            opt.GlobalFilters.Add(new MagicOnionServiceFilterDescriptor(typeof(ExceptionFilterAttribute)));
            opt.GlobalFilters.Add(new MagicOnionServiceFilterDescriptor(typeof(AuthenticationFilterAttribute)));
            opt.GlobalFilters.Add(new MagicOnionServiceFilterDescriptor(typeof(UserLockFilterAttribute)));
            opt.GlobalFilters.Add(new MagicOnionServiceFilterDescriptor(typeof(UnitOfWorkFilterAttribute)));
        });

        return services;
    }

    public static IServiceCollection AddAppServicesScoped(this IServiceCollection services)
    {
        services.AddScoped<IUserAppService, UserAppService>();
        services.AddScoped<IPostCommitQueue, PostCommitQueue>();
        services.AddScoped<SaveChangesInterceptor, PostCommitRedisInterceptor>();
        return services;
    }

    public static IServiceCollection AddAppServicesSingleton(this IServiceCollection services)
    {
        services.AddSingleton<ILockAppService, LockAppService>();
        services.AddSingleton<IJwtAppService, JwtAppService>();
        return services;
    }
}