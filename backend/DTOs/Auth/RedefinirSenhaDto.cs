using System.ComponentModel.DataAnnotations;

namespace AgentesDaAlegria.API.DTOs.Auth;

public record RedefinirSenhaDto(
    [Required, EmailAddress] string Email,
    [Required] string Token,
    [Required, MinLength(6)] string NovaSenha
);