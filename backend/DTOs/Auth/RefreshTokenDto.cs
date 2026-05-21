using System.ComponentModel.DataAnnotations;

namespace AgentesDaAlegria.API.DTOs.Auth;

public record RefreshTokenDto(
    [Required] string AccessToken,
    [Required] string RefreshToken
);