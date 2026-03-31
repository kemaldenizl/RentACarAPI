using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Security.Application.Abstractions.Authentication;
using Security.Application.Abstractions.Persistence;
using Security.Application.Abstractions.Security;
using Security.Application.Abstractions.Time;
using Security.Application.Abstractions.UnitOfWork;
using Security.Infrastructure.Persistence;
using Security.Infrastructure.Persistence.Repositories;
using Security.Infrastructure.Persistence.Seed;
using Security.Infrastructure.Security;
using Security.Infrastructure.Security.Jwt;

namespace Security.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Connection string 'Postgres' was not found.");

        services.AddDbContext<SecurityDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsHistoryTable("__ef_migrations_history", "security");
            });

            options.UseOpenIddict();
        });

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<IdentitySeedOptions>(configuration.GetSection(IdentitySeedOptions.SectionName));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IRefreshSessionRepository, RefreshSessionRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IRefreshTokenGenerator, RefreshTokenGenerator>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        services.AddSingleton<PasswordHasher>();
        services.AddScoped<IdentitySeeder>();

        services.AddSingleton<ITokenGenerator, JwtTokenGenerator>();

        return services;
    }
}