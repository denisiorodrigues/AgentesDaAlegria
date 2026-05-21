namespace AgentesDaAlegria.API.Models;

public class Presenca
{
    public int Id { get; set; }
    public Guid VoluntarioId { get; set; }
    public Voluntario Voluntario { get; set; } = null!;
    public int EventoId { get; set; }
    public Evento Evento { get; set; } = null!;
    public bool Presente { get; set; }
    public DateTime RegistradoEm { get; set; } = DateTime.UtcNow;
    public Guid RegistradoPorId { get; set; }
}
