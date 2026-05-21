using AgentesDaAlegria.API.Data;
using AgentesDaAlegria.API.DTOs.Auth;
using AgentesDaAlegria.API.Models;
using AgentesDaAlegria.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgentesDaAlegria.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    UserManager<Voluntario> userManager,
    ITokenService tokenService,
    AppDbContext db,
    IConfiguration configuration) : ControllerBase
{
    /// <summary>AU-01 Cadastro de voluntário</summary>
    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] RegistroDto dto)
    {
        var voluntario = new Voluntario
        {
            UserName = dto.Email,
            Email = dto.Email,
            NomeCompleto = dto.NomeCompleto,
            Apelido = dto.Apelido,
            DataNascimento = dto.DataNascimento,
            Telefone = dto.Telefone,
            Instagram = dto.Instagram,
            Endereco = dto.Endereco,
            Perfil = PerfilVoluntario.Voluntario
        };

        var resultado = await userManager.CreateAsync(voluntario, dto.Senha);
        if (!resultado.Succeeded)
            return BadRequest(resultado.Errors.Select(e => e.Description));

        return Created($"/api/voluntarios/{voluntario.Id}", new { voluntario.Id, voluntario.Email });
    }

    /// <summary>AU-02 Login</summary>
    [HttpPost("login")]
    public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginDto dto)
    {
        var voluntario = await userManager.FindByEmailAsync(dto.Email);
        if (voluntario is null || !await userManager.CheckPasswordAsync(voluntario, dto.Senha))
            return Unauthorized("E-mail ou senha inválidos.");

        return Ok(await EmitirTokens(voluntario));
    }

    /// <summary>AU-02 Renovar access token (refresh)</summary>
    [HttpPost("refresh")]
    public async Task<ActionResult<TokenResponseDto>> Refresh([FromBody] RefreshTokenDto dto)
    {
        var principal = tokenService.ObterPrincipalDoTokenExpirado(dto.AccessToken);
        if (principal is null)
            return Unauthorized("Access token inválido.");

        var voluntarioId = Guid.Parse(principal.FindFirst("sub")!.Value);

        var refresh = await db.TokensAtualizacao
            .FirstOrDefaultAsync(t =>
                t.VoluntarioId == voluntarioId &&
                t.Token == dto.RefreshToken &&
                !t.Revogado &&
                t.Expiracao > DateTime.UtcNow);

        if (refresh is null)
            return Unauthorized("Refresh token inválido ou expirado.");

        var voluntario = await userManager.FindByIdAsync(voluntarioId.ToString());
        if (voluntario is null)
            return Unauthorized();

        refresh.Revogado = true;
        await db.SaveChangesAsync();

        return Ok(await EmitirTokens(voluntario));
    }

    /// <summary>AU-03 Solicitar redefinição de senha</summary>
    [HttpPost("esqueci-senha")]
    public async Task<IActionResult> EsqueciSenha([FromBody] EsqueciSenhaDto dto)
    {
        var voluntario = await userManager.FindByEmailAsync(dto.Email);

        // Sempre retorna 200 para não revelar se o e-mail existe
        if (voluntario is null)
            return Ok("Se o e-mail estiver cadastrado, você receberá as instruções em breve.");

        var token = await userManager.GeneratePasswordResetTokenAsync(voluntario);

        // TODO: enviar e-mail com o token (integração com serviço de e-mail - Épico Notificações)
        var resposta = new { Mensagem = "Se o e-mail estiver cadastrado, você receberá as instruções em breve." };

        if (HttpContext.RequestServices
                .GetRequiredService<IWebHostEnvironment>().IsDevelopment())
            return Ok(new { resposta.Mensagem, TokenDesenvolvimento = token });

        return Ok(resposta);
    }

    /// <summary>AU-03 Redefinir senha com token</summary>
    [HttpPost("redefinir-senha")]
    public async Task<IActionResult> RedefinirSenha([FromBody] RedefinirSenhaDto dto)
    {
        var voluntario = await userManager.FindByEmailAsync(dto.Email);
        if (voluntario is null)
            return BadRequest("Token inválido ou expirado.");

        var resultado = await userManager.ResetPasswordAsync(voluntario, dto.Token, dto.NovaSenha);
        if (!resultado.Succeeded)
            return BadRequest(resultado.Errors.Select(e => e.Description));

        // Revoga todos os refresh tokens ao redefinir senha
        var tokensAtivos = await db.TokensAtualizacao
            .Where(t => t.VoluntarioId == voluntario.Id && !t.Revogado)
            .ToListAsync();
        foreach (var t in tokensAtivos) t.Revogado = true;

        return Ok("Senha redefinida com sucesso.");
    }

    /// <summary>AU-04 Admin cria voluntário</summary>
    [Authorize]
    [HttpPost("criar-voluntario")]
    public async Task<IActionResult> CriarVoluntario([FromBody] RegistroDto dto)
    {
        var solicitante = await userManager.GetUserAsync(User);
        if (solicitante is null || !solicitante.EhAdministrador())
            return Forbid();

        var voluntario = new Voluntario
        {
            UserName = dto.Email,
            Email = dto.Email,
            NomeCompleto = dto.NomeCompleto,
            Apelido = dto.Apelido,
            DataNascimento = dto.DataNascimento,
            Telefone = dto.Telefone,
            Instagram = dto.Instagram,
            Endereco = dto.Endereco,
            Perfil = PerfilVoluntario.Voluntario
        };

        var resultado = await userManager.CreateAsync(voluntario, dto.Senha);
        if (!resultado.Succeeded)
            return BadRequest(resultado.Errors.Select(e => e.Description));

        // TODO: enviar e-mail de boas-vindas (Épico Notificações)

        return Created($"/api/voluntarios/{voluntario.Id}", new { voluntario.Id, voluntario.Email });
    }

    private async Task<TokenResponseDto> EmitirTokens(Voluntario voluntario)
    {
        var accessToken = tokenService.GerarAccessToken(voluntario);
        var refreshToken = tokenService.GerarRefreshToken();
        var expiracaoRefresh = DateTime.UtcNow.AddDays(
            configuration.GetValue<int>("Jwt:ExpiracaoRefreshDias"));
        var expiracaoAccess = DateTime.UtcNow.AddMinutes(
            configuration.GetValue<int>("Jwt:ExpiracaoMinutos"));

        db.TokensAtualizacao.Add(new TokenAtualizacao
        {
            VoluntarioId = voluntario.Id,
            Token = refreshToken,
            Expiracao = expiracaoRefresh
        });

        await db.SaveChangesAsync();

        return new TokenResponseDto(accessToken, refreshToken, expiracaoAccess);
    }
}