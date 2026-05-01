using Microsoft.AspNetCore.Identity;

namespace AgentesDaAlegria.API.Models;

public class Voluntario : IdentityUser<Guid>
{
    public string NomeCompleto { get; set; } = string.Empty;
    public string Apelido { get; set; } = string.Empty;
    public DateOnly DataNascimento { get; set; }
    public string Telefone { get; set; } = string.Empty;
    public string? Instagram { get; set; }
    public string? Endereco { get; set; }
    public PerfilVoluntario Perfil { get; set; } = PerfilVoluntario.Voluntario;
    public DateTime? DataUltimoEvento { get; set; }

    public ICollection<Inscricao> Inscricoes { get; set; } = [];
    public ICollection<Presenca> Presencas { get; set; } = [];

    public int TotalEventosParticipados =>
        Presencas.Count(p => p.Presente);

    public bool EhAdministrador() => Perfil == PerfilVoluntario.Admin;

    public bool PodeGerenciarEvento(Evento evento) =>
        EhAdministrador() || evento.CoordenadorId == Id;
}

public enum PerfilVoluntario
{
    Voluntario,
    LiderEquipe,
    Admin
}
