using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AgentesDaAlegria.API.Models;
using Microsoft.IdentityModel.Tokens;

namespace AgentesDaAlegria.API.Services;

public class TokenService(IConfiguration configuration) : ITokenService
{
    public string GerarAccessToken(Voluntario voluntario)
    {
        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Chave"]!));
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);
        var expiracao = DateTime.UtcNow.AddMinutes(
            configuration.GetValue<int>("Jwt:ExpiracaoMinutos"));

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, voluntario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, voluntario.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("perfil", voluntario.Perfil.ToString()),
            new Claim("nomeCompleto", voluntario.NomeCompleto)
        };

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Emissor"],
            audience: configuration["Jwt:Audiencia"],
            claims: claims,
            expires: expiracao,
            signingCredentials: credenciais);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GerarRefreshToken()
    {
        var bytes = new byte[64];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }

    public ClaimsPrincipal? ObterPrincipalDoTokenExpirado(string token)
    {
        var parametros = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false, // ignora expiração propositalmente
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Emissor"],
            ValidAudience = configuration["Jwt:Audiencia"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Jwt:Chave"]!))
        };

        var handler = new JwtSecurityTokenHandler();
        var principal = handler.ValidateToken(token, parametros, out var tokenValidado);

        if (tokenValidado is not JwtSecurityToken jwt ||
            !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
            return null;

        return principal;
    }
}