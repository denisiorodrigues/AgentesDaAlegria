using System.Net;
using System.Net.Http.Json;
using AgentesDaAlegria.API.DTOs.Auth;
using AgentesDaAlegria.Tests.Helpers;

namespace AgentesDaAlegria.Tests.Integration;

public class AuthIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    // ── AU-01: Registrar ───────────────────────────────────────────────────────

    [Fact]
    public async Task Registrar_DeveRetornar201_QuandoDadosValidos()
    {
        var dto = VoluntarioBuilder.GerarRegistroDto();

        var response = await _client.PostAsJsonAsync("/api/auth/registrar", dto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Registrar_DeveRetornarIdEEmail_QuandoCriadoComSucesso()
    {
        var dto = VoluntarioBuilder.GerarRegistroDto();

        var response = await _client.PostAsJsonAsync("/api/auth/registrar", dto);
        var body = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();

        Assert.NotNull(body);
        Assert.True(body.ContainsKey("id"));
        Assert.True(body.ContainsKey("email"));
    }

    [Fact]
    public async Task Registrar_DeveRetornar400_QuandoEmailJaCadastrado()
    {
        var dto = VoluntarioBuilder.GerarRegistroDto();
        await _client.PostAsJsonAsync("/api/auth/registrar", dto);

        // Segundo registro com o mesmo e-mail
        var response = await _client.PostAsJsonAsync("/api/auth/registrar", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Registrar_DeveRetornar400_QuandoSenhaMenorQue6Caracteres()
    {
        var dto = VoluntarioBuilder.GerarRegistroDto(senha: "123");

        var response = await _client.PostAsJsonAsync("/api/auth/registrar", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Registrar_DeveRetornar400_QuandoEmailInvalido()
    {
        var dto = VoluntarioBuilder.GerarRegistroDto(email: "nao-e-um-email");

        var response = await _client.PostAsJsonAsync("/api/auth/registrar", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ── AU-02: Login ───────────────────────────────────────────────────────────

    [Fact]
    public async Task Login_DeveRetornar200_ComTokens_QuandoCredenciaisValidas()
    {
        var senha = "Senha@123";
        var dto = VoluntarioBuilder.GerarRegistroDto(senha: senha);
        await _client.PostAsJsonAsync("/api/auth/registrar", dto);

        var response = await _client.PostAsJsonAsync("/api/auth/login",
            new LoginDto(dto.Email, senha));
        var tokens = await response.Content.ReadFromJsonAsync<TokenResponseDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(tokens);
        Assert.False(string.IsNullOrWhiteSpace(tokens.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(tokens.RefreshToken));
        Assert.True(tokens.Expiracao > DateTime.UtcNow);
    }

    [Fact]
    public async Task Login_DeveRetornar401_QuandoSenhaErrada()
    {
        var dto = VoluntarioBuilder.GerarRegistroDto(senha: "SenhaCorreta@1");
        await _client.PostAsJsonAsync("/api/auth/registrar", dto);

        var response = await _client.PostAsJsonAsync("/api/auth/login",
            new LoginDto(dto.Email, "SenhaErrada@1"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_DeveRetornar401_QuandoEmailNaoCadastrado()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login",
            new LoginDto("inexistente@teste.com", "Senha@123"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // ── AU-02: Refresh token ───────────────────────────────────────────────────

    [Fact]
    public async Task Refresh_DeveRetornarNovosTokens_QuandoRefreshValido()
    {
        var tokens = await RegistrarELogar();

        var response = await _client.PostAsJsonAsync("/api/auth/refresh",
            new RefreshTokenDto(tokens.AccessToken, tokens.RefreshToken));
        var novosTokens = await response.Content.ReadFromJsonAsync<TokenResponseDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(novosTokens);
        Assert.NotEqual(tokens.AccessToken, novosTokens.AccessToken);
        Assert.NotEqual(tokens.RefreshToken, novosTokens.RefreshToken);
    }

    [Fact]
    public async Task Refresh_DeveRetornar401_QuandoRefreshTokenInvalido()
    {
        var tokens = await RegistrarELogar();

        var response = await _client.PostAsJsonAsync("/api/auth/refresh",
            new RefreshTokenDto(tokens.AccessToken, "refresh-token-invalido"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Refresh_DeveRetornar401_QuandoRefreshTokenJaFoiUsado()
    {
        var tokens = await RegistrarELogar();

        // Usa o refresh token uma vez (revoga o original)
        await _client.PostAsJsonAsync("/api/auth/refresh",
            new RefreshTokenDto(tokens.AccessToken, tokens.RefreshToken));

        // Tenta usar o mesmo refresh token novamente
        var response = await _client.PostAsJsonAsync("/api/auth/refresh",
            new RefreshTokenDto(tokens.AccessToken, tokens.RefreshToken));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // ── AU-03: Esqueci a senha ─────────────────────────────────────────────────

    [Fact]
    public async Task EsqueciSenha_DeveRetornar200_IndependenteDeEmailExistir()
    {
        var responseComEmail = await _client.PostAsJsonAsync("/api/auth/esqueci-senha",
            new EsqueciSenhaDto("existe@teste.com"));

        var responseSemEmail = await _client.PostAsJsonAsync("/api/auth/esqueci-senha",
            new EsqueciSenhaDto("naoexiste@teste.com"));

        Assert.Equal(HttpStatusCode.OK, responseComEmail.StatusCode);
        Assert.Equal(HttpStatusCode.OK, responseSemEmail.StatusCode);
    }

    [Fact]
    public async Task EsqueciSenha_DeveRetornar400_QuandoEmailInvalido()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/esqueci-senha",
            new EsqueciSenhaDto("nao-e-email"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ── AU-03: Redefinir senha ─────────────────────────────────────────────────

    [Fact]
    public async Task RedefinirSenha_DeveRetornar400_QuandoTokenInvalido()
    {
        var dto = VoluntarioBuilder.GerarRegistroDto();
        await _client.PostAsJsonAsync("/api/auth/registrar", dto);

        var response = await _client.PostAsJsonAsync("/api/auth/redefinir-senha",
            new RedefinirSenhaDto(dto.Email, "token-invalido", "NovaSenha@123"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task RedefinirSenha_DeveRetornar400_QuandoEmailNaoExiste()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/redefinir-senha",
            new RedefinirSenhaDto("naoexiste@teste.com", "qualquer-token", "NovaSenha@123"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ── AU-04: Admin cria voluntário ───────────────────────────────────────────

    [Fact]
    public async Task CriarVoluntario_DeveRetornar403_QuandoVoluntarioComum()
    {
        var tokens = await RegistrarELogar();
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokens.AccessToken);

        var dto = VoluntarioBuilder.GerarRegistroDto();
        var response = await _client.PostAsJsonAsync("/api/auth/criar-voluntario", dto);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task CriarVoluntario_DeveRetornar401_SemAutenticacao()
    {
        var dto = VoluntarioBuilder.GerarRegistroDto();

        var response = await _client.PostAsJsonAsync("/api/auth/criar-voluntario", dto);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    private async Task<TokenResponseDto> RegistrarELogar()
    {
        var senha = "Senha@123";
        var dto = VoluntarioBuilder.GerarRegistroDto(senha: senha);
        await _client.PostAsJsonAsync("/api/auth/registrar", dto);

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login",
            new LoginDto(dto.Email, senha));

        return (await loginResponse.Content.ReadFromJsonAsync<TokenResponseDto>())!;
    }
}