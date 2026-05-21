using System.ComponentModel.DataAnnotations;

namespace AgentesDaAlegria.API.DTOs.Auth;

public record LoginDto(
    [Required, EmailAddress] string Email,
    [Required] string Senha
);