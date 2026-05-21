using System.ComponentModel.DataAnnotations;

namespace AgentesDaAlegria.API.DTOs.Auth;

public record RegistroDto(
    [Required] string NomeCompleto,
    [Required] string Apelido,
    [Required] DateOnly DataNascimento,
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Senha,
    [Required] string Telefone,
    string? Instagram,
    string? Endereco
);