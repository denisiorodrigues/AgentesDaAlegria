using AgentesDaAlegria.API.Models;

namespace AgentesDaAlegria.Tests.Helpers;

/// <summary>
/// Builder para criar instâncias de Evento nos testes com valores padrão sensatos.
/// Evita repetição e torna os testes legíveis: só o que importa pro cenário é configurado.
/// </summary>
public class EventoBuilder
{
    private int _totalVagas = 10;
    private DateTime _data = DateTime.UtcNow.AddDays(7);
    private int _horasLimiteCancelamento = 24;
    private List<Inscricao> _inscricoes = [];

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

    public Evento Build() => new()
    {
        Id = 1,
        Nome = "Evento Teste",
        Data = _data,
        Local = "Local Teste",
        TotalVagas = _totalVagas,
        HorasLimiteCancelamento = _horasLimiteCancelamento,
        CoordenadorId = 99,
        Inscricoes = _inscricoes
    };
}
