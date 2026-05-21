using System.ComponentModel.DataAnnotations;

namespace AgentesDaAlegria.API.DTOs.Auth;

public record EsqueciSenhaDto(
    [Required, EmailAddress] string Email
);