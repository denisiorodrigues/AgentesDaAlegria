using AgentesDaAlegria.API.Models;
using AgentesDaAlegria.Tests.Helpers;

namespace AgentesDaAlegria.Tests.Models;

public class EventoTests
{
    // ── VagasOcupadas ──────────────────────────────────────────────────────────

    [Fact]
    public void VagasOcupadas_ContaApenasInscricoesConfirmadas()
    {
        var inscricoes = new List<Inscricao>
        {
            new() { Status = StatusInscricao.Confirmado },
            new() { Status = StatusInscricao.Confirmado },
            new() { Status = StatusInscricao.FilaEspera },
            new() { Status = StatusInscricao.Cancelado }
        };

        var evento = new EventoBuilder().ComVagas(10).ComInscricoes(inscricoes).Build();

        Assert.Equal(2, evento.VagasOcupadas);
    }

    // ── EstaLotado ─────────────────────────────────────────────────────────────

    [Fact]
    public void EstaLotado_RetornaFalse_QuandoHaVagasDisponiveis()
    {
        var inscricoes = new List<Inscricao>
        {
            new() { Status = StatusInscricao.Confirmado }
        };

        var evento = new EventoBuilder().ComVagas(5).ComInscricoes(inscricoes).Build();

        Assert.False(evento.EstaLotado());
    }

    [Fact]
    public void EstaLotado_RetornaTrue_QuandoTodasAsVagasEstaoOcupadas()
    {
        var inscricoes = Enumerable.Range(0, 3)
            .Select(_ => new Inscricao { Status = StatusInscricao.Confirmado })
            .ToList();

        var evento = new EventoBuilder().ComVagas(3).ComInscricoes(inscricoes).Build();

        Assert.True(evento.EstaLotado());
    }

    // ── AceitaInscricoes ───────────────────────────────────────────────────────

    [Fact]
    public void AceitaInscricoes_RetornaTrue_QuandoHaVagaEEventoNaoComecou()
    {
        var evento = new EventoBuilder()
            .ComVagas(10)
            .ComData(DateTime.UtcNow.AddDays(1))
            .Build();

        Assert.True(evento.AceitaInscricoes(DateTime.UtcNow));
    }

    [Fact]
    public void AceitaInscricoes_RetornaFalse_QuandoEventoEstaLotado()
    {
        var inscricoes = new List<Inscricao>
        {
            new() { Status = StatusInscricao.Confirmado }
        };

        var evento = new EventoBuilder()
            .ComVagas(1)
            .ComData(DateTime.UtcNow.AddDays(1))
            .ComInscricoes(inscricoes)
            .Build();

        Assert.False(evento.AceitaInscricoes(DateTime.UtcNow));
    }

    [Fact]
    public void AceitaInscricoes_RetornaFalse_QuandoEventoJaOcorreu()
    {
        var evento = new EventoBuilder()
            .ComVagas(10)
            .ComData(DateTime.UtcNow.AddDays(-1))
            .Build();

        Assert.False(evento.AceitaInscricoes(DateTime.UtcNow));
    }

    // ── PodeCancelar ───────────────────────────────────────────────────────────

    [Theory]
    [InlineData(24, 25)]  // limite=24h, faltam 25h → pode cancelar
    [InlineData(48, 49)]  // limite=48h, faltam 49h → pode cancelar
    public void PodeCancelar_RetornaTrue_QuandoDentroDoPrazo(int limiteHoras, int horasAteEvento)
    {
        var agora = DateTime.UtcNow;
        var evento = new EventoBuilder()
            .ComData(agora.AddHours(horasAteEvento))
            .ComLimiteCancelamento(limiteHoras)
            .Build();

        Assert.True(evento.PodeCancelar(agora));
    }

    [Theory]
    [InlineData(24, 23)]  // limite=24h, faltam apenas 23h → não pode
    [InlineData(24, 0)]   // evento já ocorreu → não pode
    public void PodeCancelar_RetornaFalse_QuandoForaDoPrazo(int limiteHoras, int horasAteEvento)
    {
        var agora = DateTime.UtcNow;
        var evento = new EventoBuilder()
            .ComData(agora.AddHours(horasAteEvento))
            .ComLimiteCancelamento(limiteHoras)
            .Build();

        Assert.False(evento.PodeCancelar(agora));
    }
}
