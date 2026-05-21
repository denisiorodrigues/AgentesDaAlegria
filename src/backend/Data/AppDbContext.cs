using AgentesDaAlegria.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AgentesDaAlegria.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<Voluntario, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Voluntario> Voluntarios => Set<Voluntario>();
    public DbSet<Evento> Eventos => Set<Evento>();
    public DbSet<Inscricao> Inscricoes => Set<Inscricao>();
    public DbSet<Presenca> Presencas => Set<Presenca>();
    public DbSet<Aviso> Avisos => Set<Aviso>();
    public DbSet<TokenAtualizacao> TokensAtualizacao => Set<TokenAtualizacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Remove prefixo "AspNet" dos nomes de tabela do Identity
        modelBuilder.Entity<Voluntario>().ToTable("Voluntarios");
        modelBuilder.Entity<IdentityRole<Guid>>().ToTable("Perfis");
        modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("VoluntarioPerfis");
        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("VoluntarioClaims");
        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("VoluntarioLogins");
        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("VoluntarioTokens");
        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("PerfisClaims");

        modelBuilder.Entity<Inscricao>()
            .HasIndex(i => new { i.VoluntarioId, i.EventoId })
            .IsUnique();

        modelBuilder.Entity<Presenca>()
            .HasIndex(p => new { p.VoluntarioId, p.EventoId })
            .IsUnique();
    }
}