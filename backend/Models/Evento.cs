namespace AgentesDaAlegria.API.Models;

public class Evento
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public DateTime Data { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFim { get; set; }
    public string Local { get; set; } = string.Empty;
    public string? Tipo { get; set; }
    public string? Descricao { get; set; }
    public int TotalVagas { get; set; }
    public int HorasLimiteCancelamento { get; set; } = 24;

    public Guid CoordenadorId { get; set; }
    public Voluntario Coordenador { get; set; } = null!;

    public ICollection<Inscricao> Inscricoes { get; set; } = [];
    public ICollection<Aviso> Avisos { get; set; } = [];
    public ICollection<Presenca> Presencas { get; set; } = [];

    public int VagasOcupadas =>
        Inscricoes.Count(i => i.Status == StatusInscricao.Confirmado);

    public bool EstaLotado() => VagasOcupadas >= TotalVagas;

    // Voluntário pode se inscrever se houver vaga e o evento não começou
    public bool AceitaInscricoes(DateTime agora) =>
        !EstaLotado() && agora < Data;

    // Cancelamento só é permitido antes do prazo configurado
    public bool PodeCancelar(DateTime agora) =>
        agora < Data.AddHours(-HorasLimiteCancelamento);
}
