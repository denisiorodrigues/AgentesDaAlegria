using AgentesDaAlegria.API.Models;
using AgentesDaAlegria.Tests.Helpers;

namespace AgentesDaAlegria.Tests.Models;

public class InscricaoTests
{
    // ── EstaAtiva ──────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(StatusInscricao.Confirmado)]
    [InlineData(StatusInscricao.FilaEspera)]
    public void EstaAtiva_RetornaTrue_ParaStatusAtivos(StatusInscricao status)
    {
        var inscricao = new Inscricao { Status = status };

        Assert.True(inscricao.EstaAtiva());
    }

    [Fact]
    public void EstaAtiva_RetornaFalse_QuandoCancelada()
    {
        var inscricao = new Inscricao { Status = StatusInscricao.Cancelado };

        Assert.False(inscricao.EstaAtiva());
    }

    // ── Promover ───────────────────────────────────────────────────────────────

    [Fact]
    public void Promover_AlteraStatusParaConfirmado_ELimpaPosicao()
    {
        var inscricao = new Inscricao
        {
            Status = StatusInscricao.FilaEspera,
            PosicaoFilaEspera = 3
        };

        inscricao.Promover();

        Assert.Equal(StatusInscricao.Confirmado, inscricao.Status);
        Assert.Null(inscricao.PosicaoFilaEspera);
    }

    [Theory]
    [InlineData(StatusInscricao.Confirmado)]
    [InlineData(StatusInscricao.Cancelado)]
    public void Promover_LancaExcecao_SeNaoEstiverEmFilaDeEspera(StatusInscricao status)
    {
        var inscricao = new Inscricao { Status = status };

        Assert.Throws<InvalidOperationException>(() => inscricao.Promover());
    }

    // ── PodeCancelar ───────────────────────────────────────────────────────────

    [Fact]
    public void PodeCancelar_RetornaTrue_QuandoAtivaEDentroDoPrazo()
    {
        var agora = DateTime.UtcNow;
        var evento = new EventoBuilder()
            .ComData(agora.AddHours(48))
            .ComLimiteCancelamento(24)
            .Build();

        var inscricao = new Inscricao { Status = StatusInscricao.Confirmado, Evento = evento };

        Assert.True(inscricao.PodeCancelar(agora));
    }

    [Fact]
    public void PodeCancelar_RetornaFalse_QuandoCancelada()
    {
        var agora = DateTime.UtcNow;
        var evento = new EventoBuilder()
            .ComData(agora.AddHours(48))
            .ComLimiteCancelamento(24)
            .Build();

        var inscricao = new Inscricao { Status = StatusInscricao.Cancelado, Evento = evento };

        Assert.False(inscricao.PodeCancelar(agora));
    }

    [Fact]
    public void PodeCancelar_RetornaFalse_QuandoForaDoPrazoDoEvento()
    {
        var agora = DateTime.UtcNow;
        var evento = new EventoBuilder()
            .ComData(agora.AddHours(10))  // faltam 10h, mas limite é 24h
            .ComLimiteCancelamento(24)
            .Build();

        var inscricao = new Inscricao { Status = StatusInscricao.Confirmado, Evento = evento };

        Assert.False(inscricao.PodeCancelar(agora));
    }
}
