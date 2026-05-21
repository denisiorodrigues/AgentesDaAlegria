using AgentesDaAlegria.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AgentesDaAlegria.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Voluntario> Voluntarios => Set<Voluntario>();
    public DbSet<Evento> Eventos => Set<Evento>();
    public DbSet<Inscricao> Inscricoes => Set<Inscricao>();
    public DbSet<Presenca> Presencas => Set<Presenca>();
    public DbSet<Aviso> Avisos => Set<Aviso>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Índice único: um voluntário só pode ter uma inscrição ativa por evento
        modelBuilder.Entity<Inscricao>()
            .HasIndex(i => new { i.VoluntarioId, i.EventoId })
            .IsUnique();

        // Índice único: um voluntário tem apenas um registro de presença por evento
        modelBuilder.Entity<Presenca>()
            .HasIndex(p => new { p.VoluntarioId, p.EventoId })
            .IsUnique();

        modelBuilder.Entity<Voluntario>()
            .HasIndex(v => v.Email)
            .IsUnique();
    }
}
