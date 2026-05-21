using AgentesDaAlegria.API.Models;
using AgentesDaAlegria.Tests.Helpers;

namespace AgentesDaAlegria.Tests.Models;

public class VoluntarioTests
{
    // ── TotalEventosParticipados ───────────────────────────────────────────────

    [Fact]
    public void TotalEventosParticipados_ContaApenasPresencasConfirmadas()
    {
        var voluntario = new Voluntario
        {
            Presencas =
            [
                new Presenca { Presente = true },
                new Presenca { Presente = true },
                new Presenca { Presente = false }
            ]
        };

        Assert.Equal(2, voluntario.TotalEventosParticipados);
    }

    [Fact]
    public void TotalEventosParticipados_RetornaZero_SemPresencas()
    {
        var voluntario = new Voluntario();

        Assert.Equal(0, voluntario.TotalEventosParticipados);
    }

    // ── EhAdministrador ────────────────────────────────────────────────────────

    [Fact]
    public void EhAdministrador_RetornaTrue_ParaPerfilAdmin()
    {
        var voluntario = new Voluntario { Perfil = PerfilVoluntario.Admin };

        Assert.True(voluntario.EhAdministrador());
    }

    [Theory]
    [InlineData(PerfilVoluntario.Voluntario)]
    [InlineData(PerfilVoluntario.LiderEquipe)]
    public void EhAdministrador_RetornaFalse_ParaOutrosPerfis(PerfilVoluntario perfil)
    {
        var voluntario = new Voluntario { Perfil = perfil };

        Assert.False(voluntario.EhAdministrador());
    }

    // ── PodeGerenciarEvento ────────────────────────────────────────────────────

    [Fact]
    public void PodeGerenciarEvento_RetornaTrue_ParaAdministrador()
    {
        var admin = new Voluntario { Id = 1, Perfil = PerfilVoluntario.Admin };
        var evento = new EventoBuilder().Build(); // CoordenadorId = 99

        Assert.True(admin.PodeGerenciarEvento(evento));
    }

    [Fact]
    public void PodeGerenciarEvento_RetornaTrue_ParaCoordenadorDoEvento()
    {
        var coordenador = new Voluntario { Id = 99, Perfil = PerfilVoluntario.Voluntario };
        var evento = new EventoBuilder().Build(); // CoordenadorId = 99

        Assert.True(coordenador.PodeGerenciarEvento(evento));
    }

    [Fact]
    public void PodeGerenciarEvento_RetornaFalse_ParaVoluntarioComum()
    {
        var voluntario = new Voluntario { Id = 42, Perfil = PerfilVoluntario.Voluntario };
        var evento = new EventoBuilder().Build(); // CoordenadorId = 99

        Assert.False(voluntario.PodeGerenciarEvento(evento));
    }
}
