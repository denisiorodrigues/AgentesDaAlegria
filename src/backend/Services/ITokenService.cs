using System.Security.Claims;
using AgentesDaAlegria.API.Models;

namespace AgentesDaAlegria.API.Services;

public interface ITokenService
{
    string GerarAccessToken(Voluntario voluntario);
    string GerarRefreshToken();
    ClaimsPrincipal? ObterPrincipalDoTokenExpirado(string token);
}