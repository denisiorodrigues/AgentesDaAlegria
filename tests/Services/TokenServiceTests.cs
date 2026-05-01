using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AgentesDaAlegria.API.Services;
using AgentesDaAlegria.Tests.Helpers;
using Microsoft.Extensions.Configuration;

namespace AgentesDaAlegria.Tests.Services;

public class TokenServiceTests
{
    private readonly ITokenService _service;

    private static IConfiguration CriarConfig() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Chave"] = "chave-de-teste-deve-ter-minimo-32-chars-ok!",
                ["Jwt:Emissor"] = "AgentesDaAlegria",
                ["Jwt:Audiencia"] = "AgentesDaAlegria",
                ["Jwt:ExpiracaoMinutos"] = "60",
                ["Jwt:ExpiracaoRefreshDias"] = "7"
            })
            .Build();

    public TokenServiceTests()
    {
        _service = new TokenService(CriarConfig());
    }

    // ── GerarAccessToken ───────────────────────────────────────────────────────

    [Fact]
    public void GerarAccessToken_DeveRetornarJwtValido()
    {
        var voluntario = VoluntarioBuilder.Gerar();

        var token = _service.GerarAccessToken(voluntario);

        Assert.False(string.IsNullOrWhiteSpace(token));
        var handler = new JwtSecurityTokenHandler();
        Assert.True(handler.CanReadToken(token));
    }

    [Fact]
    public void GerarAccessToken_DeveConterClaimsCorretos()
    {
        var voluntario = VoluntarioBuilder.Gerar();

        var token = _service.GerarAccessToken(voluntario);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        Assert.Equal(voluntario.Id.ToString(), jwt.Subject);
        Assert.Contains(jwt.Claims, c => c.Type == JwtRegisteredClaimNames.Email && c.Value == voluntario.Email);
        Assert.Contains(jwt.Claims, c => c.Type == "perfil" && c.Value == voluntario.Perfil.ToString());
        Assert.Contains(jwt.Claims, c => c.Type == "nomeCompleto" && c.Value == voluntario.NomeCompleto);
    }

    [Fact]
    public void GerarAccessToken_DeveConterIssuerEAudienceCorretos()
    {
        var voluntario = VoluntarioBuilder.Gerar();

        var token = _service.GerarAccessToken(voluntario);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        Assert.Equal("AgentesDaAlegria", jwt.Issuer);
        Assert.Contains("AgentesDaAlegria", jwt.Audiences);
    }

    [Fact]
    public void GerarAccessToken_DeveExpirarEm60Minutos()
    {
        var voluntario = VoluntarioBuilder.Gerar();

        var antes = DateTime.UtcNow;
        var token = _service.GerarAccessToken(voluntario);
        var depois = DateTime.UtcNow;

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        Assert.True(jwt.ValidTo >= antes.AddMinutes(59));
        Assert.True(jwt.ValidTo <= depois.AddMinutes(61));
    }

    // ── GerarRefreshToken ──────────────────────────────────────────────────────

    [Fact]
    public void GerarRefreshToken_DeveRetornarStringBase64NaoVazia()
    {
        var token = _service.GerarRefreshToken();

        Assert.False(string.IsNullOrWhiteSpace(token));
        var bytes = Convert.FromBase64String(token); // lança se não for Base64
        Assert.Equal(64, bytes.Length);
    }

    [Fact]
    public void GerarRefreshToken_DeveSerUnicoACadaChamada()
    {
        var token1 = _service.GerarRefreshToken();
        var token2 = _service.GerarRefreshToken();

        Assert.NotEqual(token1, token2);
    }

    // ── ObterPrincipalDoTokenExpirado ──────────────────────────────────────────

    [Fact]
    public void ObterPrincipalDoTokenExpirado_DeveRetornarPrincipal_ComTokenValido()
    {
        var voluntario = VoluntarioBuilder.Gerar();
        var token = _service.GerarAccessToken(voluntario);

        var principal = _service.ObterPrincipalDoTokenExpirado(token);

        Assert.NotNull(principal);
        Assert.Equal(voluntario.Id.ToString(),
            principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? principal.FindFirst("sub")?.Value);
    }

    [Fact]
    public void ObterPrincipalDoTokenExpirado_DeveRetornarNull_ComTokenMalformado()
    {
        var principal = _service.ObterPrincipalDoTokenExpirado("token.invalido.aqui");

        Assert.Null(principal);
    }

    [Fact]
    public void ObterPrincipalDoTokenExpirado_DeveRetornarNull_ComTokenAssinadoComChaveDiferente()
    {
        var configOutraChave = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Chave"] = "outra-chave-completamente-diferente-32chars!",
                ["Jwt:Emissor"] = "AgentesDaAlegria",
                ["Jwt:Audiencia"] = "AgentesDaAlegria",
                ["Jwt:ExpiracaoMinutos"] = "60",
                ["Jwt:ExpiracaoRefreshDias"] = "7"
            })
            .Build();
        var serviceOutraChave = new TokenService(configOutraChave);
        var tokenOutraChave = serviceOutraChave.GerarAccessToken(VoluntarioBuilder.Gerar());

        // Tenta validar token assinado com outra chave usando o service original
        var principal = _service.ObterPrincipalDoTokenExpirado(tokenOutraChave);

        Assert.Null(principal);
    }
}