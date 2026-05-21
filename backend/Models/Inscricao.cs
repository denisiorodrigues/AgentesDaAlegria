namespace AgentesDaAlegria.API.Models;

public class Inscricao
{
    public int Id { get; set; }
    public Guid VoluntarioId { get; set; }
    public Voluntario Voluntario { get; set; } = null!;
    public int EventoId { get; set; }
    public Evento Evento { get; set; } = null!;
    public DateTime InscritoEm { get; set; } = DateTime.UtcNow;
    public StatusInscricao Status { get; set; } = StatusInscricao.Confirmado;
    public int? PosicaoFilaEspera { get; set; }

    public bool EstaAtiva() =>
        Status == StatusInscricao.Confirmado || Status == StatusInscricao.FilaEspera;

    public bool PodeCancelar(DateTime agora) =>
        EstaAtiva() && Evento.PodeCancelar(agora);

    // Promove da fila de espera para vaga confirmada
    public void Promover()
    {
        if (Status != StatusInscricao.FilaEspera)
            throw new InvalidOperationException("Apenas inscrições em fila de espera podem ser promovidas.");

        Status = StatusInscricao.Confirmado;
        PosicaoFilaEspera = null;
    }
}

public enum StatusInscricao
{
    Confirmado,
    FilaEspera,
    Cancelado
}
