using AgentesDaAlegria.API.Models;

namespace AgentesDaAlegria.Tests.Helpers;

public class EventoBuilder
{
    // Guid fixo usado como CoordenadorId padrão — útil para asserts de coordenador nos testes
    public static readonly Guid DefaultCoordenadorId = new("99999999-0000-0000-0000-000000000099");

    private int _totalVagas = 10;
    private DateTime _data = DateTime.UtcNow.AddDays(7);
    private int _horasLimiteCancelamento = 24;
    private List<Inscricao> _inscricoes = [];
    private Guid _coordenadorId = DefaultCoordenadorId;

    public EventoBuilder ComVagas(int vagas)
    {
        _totalVagas = vagas;
        return this;
    }

    public EventoBuilder ComData(DateTime data)
    {
        _data = data;
        return this;
    }

    public EventoBuilder ComLimiteCancelamento(int horas)
    {
        _horasLimiteCancelamento = horas;
        return this;
    }

    public EventoBuilder ComInscricoes(List<Inscricao> inscricoes)
    {
        _inscricoes = inscricoes;
        return this;
    }

    public EventoBuilder ComCoordenador(Guid coordenadorId)
    {
        _coordenadorId = coordenadorId;
        return this;
    }

    public Evento Build() => new()
    {
        Id = 1,
        Nome = "Evento Teste",
        Data = _data,
        Local = "Local Teste",
        TotalVagas = _totalVagas,
        HorasLimiteCancelamento = _horasLimiteCancelamento,
        CoordenadorId = _coordenadorId,
        Inscricoes = _inscricoes
    };
}