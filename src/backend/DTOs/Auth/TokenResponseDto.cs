namespace AgentesDaAlegria.API.DTOs.Auth;

public record TokenResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTime Expiracao
);