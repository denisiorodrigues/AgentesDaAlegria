namespace AgentesDaAlegria.API.Models;

public class TokenAtualizacao
{
    public int Id { get; set; }
    public Guid VoluntarioId { get; set; }
    public Voluntario Voluntario { get; set; } = null!;
    public string Token { get; set; } = string.Empty;
    public DateTime Expiracao { get; set; }
    public bool Revogado { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}