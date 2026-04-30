namespace AgentesDaAlegria.API.Models;

public class Aviso
{
    public int Id { get; set; }
    public int EventoId { get; set; }
    public Evento Evento { get; set; } = null!;
    public int AutorId { get; set; }
    public Voluntario Autor { get; set; } = null!;
    public string Titulo { get; set; } = string.Empty;
    public string Corpo { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
