using AgentesDaAlegria.API.Models;
using Microsoft.AspNetCore.Identity;

namespace AgentesDaAlegria.API.Data;

public static class AdminSeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration config, ILogger logger)
    {
        var userManager = services.GetRequiredService<UserManager<Voluntario>>();
        var section = config.GetSection("SeedAdmin");

        var email = section["Email"];
        if (string.IsNullOrEmpty(email) || await userManager.FindByEmailAsync(email) is not null)
            return;

        var admin = new Voluntario
        {
            UserName = email,
            Email = email,
            NomeCompleto = section["NomeCompleto"] ?? "Admin",
            Apelido = section["Apelido"] ?? "Admin",
            Telefone = section["Telefone"] ?? "00000000000",
            DataNascimento = DateOnly.FromDateTime(DateTime.UtcNow),
            Perfil = PerfilVoluntario.Admin,
            EmailConfirmed = true,
        };

        var senha = section["Senha"] ?? throw new InvalidOperationException("SeedAdmin:Senha não configurada.");

        if (senha.Length < 12)
            throw new InvalidOperationException("SeedAdmin:Senha deve ter no mínimo 12 caracteres.");

        var resultado = await userManager.CreateAsync(admin, senha);

        if (!resultado.Succeeded)
        {
            var erros = string.Join(", ", resultado.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Falha ao criar admin seed: {erros}");
        }

        logger.LogInformation("Admin seed criado: {Email}", email);
    }
}
