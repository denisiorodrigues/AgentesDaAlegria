using AgentesDaAlegria.API.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AgentesDaAlegria.Tests.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    // Banco único e isolado por instância de factory (= por classe de teste)
    private readonly string _dbName = $"TestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Evita carregar user-secrets do backend (que têm a chave JWT de dev)
        builder.UseEnvironment("Test");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Chave"] = "chave-de-teste-deve-ter-minimo-32-chars-ok!",
                ["Jwt:Emissor"] = "AgentesDaAlegria",
                ["Jwt:Audiencia"] = "AgentesDaAlegria",
                ["Jwt:ExpiracaoMinutos"] = "60",
                ["Jwt:ExpiracaoRefreshDias"] = "7"
            });
        });

        builder.ConfigureTestServices(services =>
        {
            // EF Core 8+ registra opções via IDbContextOptionsConfiguration<T> — remove ambos
            var configType = typeof(IDbContextOptionsConfiguration<AppDbContext>);
            var optionsType = typeof(DbContextOptions<AppDbContext>);

            var toRemove = services
                .Where(d => d.ServiceType == configType || d.ServiceType == optionsType)
                .ToList();

            foreach (var d in toRemove)
                services.Remove(d);

            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(_dbName));
        });
    }
}