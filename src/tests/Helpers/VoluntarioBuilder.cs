using AgentesDaAlegria.API.DTOs.Auth;
using AgentesDaAlegria.API.Models;
using Bogus;

namespace AgentesDaAlegria.Tests.Helpers;

public static class VoluntarioBuilder
{
    private static readonly Faker Fake = new("pt_BR");

    public static Voluntario Gerar(PerfilVoluntario perfil = PerfilVoluntario.Voluntario)
    {
        var email = Fake.Internet.Email();
        return new Voluntario
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email,
            NormalizedEmail = email.ToUpperInvariant(),
            NormalizedUserName = email.ToUpperInvariant(),
            NomeCompleto = Fake.Name.FullName(),
            Apelido = Fake.Internet.UserName().ToLower(),
            DataNascimento = DateOnly.FromDateTime(
                Fake.Date.Past(40, DateTime.Now.AddYears(-18))),
            Telefone = Fake.Phone.PhoneNumber("(##) #####-####"),
            Instagram = $"@{Fake.Internet.UserName()}",
            Endereco = $"{Fake.Address.StreetName()}, {Fake.Address.BuildingNumber()}",
            Perfil = perfil
        };
    }

    public static List<Voluntario> Gerar(int quantidade, PerfilVoluntario perfil = PerfilVoluntario.Voluntario) =>
        Enumerable.Range(0, quantidade).Select(_ => Gerar(perfil)).ToList();

    public static RegistroDto GerarRegistroDto(string? email = null, string? senha = null)
    {
        var emailFinal = email ?? Fake.Internet.Email();
        return new RegistroDto(
            NomeCompleto: Fake.Name.FullName(),
            Apelido: Fake.Internet.UserName().ToLower(),
            DataNascimento: DateOnly.FromDateTime(
                Fake.Date.Past(40, DateTime.Now.AddYears(-18))),
            Email: emailFinal,
            Senha: senha ?? "Senha@123",
            Telefone: Fake.Phone.PhoneNumber("(##) #####-####"),
            Instagram: $"@{Fake.Internet.UserName()}",
            Endereco: $"{Fake.Address.StreetName()}, {Fake.Address.BuildingNumber()}"
        );
    }
}