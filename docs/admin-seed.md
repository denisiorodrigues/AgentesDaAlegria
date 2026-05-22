# Admin Seed — Criação do Usuário Administrador Inicial

## Visão Geral

O projeto não possui tela de cadastro público — todo usuário é cadastrado por um administrador logado. Para quebrar esse ciclo na primeira inicialização, existe um mecanismo de **seed automático** que cria um usuário admin ao subir a aplicação, caso ele ainda não exista.

A lógica é **idempotente**: se o e-mail já estiver cadastrado no banco, o seeder retorna sem fazer nada. Reiniciar a aplicação quantas vezes quiser é seguro.

O seeder roda **apenas em Development** — em produção é no-op por design.

---

## Arquivos Envolvidos

| Arquivo | Responsabilidade |
|---|---|
| `src/backend/Data/AdminSeeder.cs` | Lógica de seed via `UserManager` |
| `src/backend/appsettings.Development.json` | E-mail e dados não sensíveis do admin de desenvolvimento |
| `src/backend/Program.cs` | Chama o seeder dentro do bloco `IsDevelopment` |

---

## Como Funciona

### 1. Configuração por ambiente

O .NET carrega arquivos de configuração em camadas, com prioridade crescente:

```
appsettings.json
  → appsettings.{Ambiente}.json
    → User Secrets (somente Development)
      → Variáveis de ambiente
        → Argumentos de linha de comando
```

A seção `SeedAdmin` existe em `appsettings.Development.json`, mas a senha fica **vazia** nesse arquivo — ela deve ser fornecida via `user-secrets` localmente ou variáveis de ambiente. O seeder lança exceção ao iniciar se a senha não estiver configurada, evitando falhas silenciosas.

O seeder só é executado quando `app.Environment.IsDevelopment()` é verdadeiro — em Staging e Production o bloco inteiro é ignorado.

### 2. Por que usar `UserManager` e não INSERT direto no banco

O ASP.NET Core Identity exige que várias colunas sejam preenchidas corretamente para o login funcionar:

- `PasswordHash` — gerado com PBKDF2 + salt aleatório
- `SecurityStamp` — usado para invalidar sessões
- `ConcurrencyStamp` — controle de concorrência
- `NormalizedEmail` / `NormalizedUserName` — usados nas queries de busca

Um `INSERT` manual ignoraria tudo isso e causaria falhas silenciosas na autenticação. O `UserManager.CreateAsync()` garante que todos esses campos são preenchidos corretamente.

### 3. Validação de senha mínima

A política global de senhas do Identity está relaxada (mínimo 6 caracteres, sem requisitos de complexidade) para facilitar o cadastro de voluntários. O `AdminSeeder` compensa isso com uma validação própria — senhas com menos de 12 caracteres são rejeitadas na inicialização.

### 4. O padrão `CreateScope`

O `UserManager` é um serviço **scoped** (existe por requisição HTTP). Na inicialização do app, fora de uma requisição, é necessário criar um scope manualmente:

```csharp
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    await AdminSeeder.SeedAsync(scope.ServiceProvider, app.Configuration, app.Logger);
}
```

O `using` garante que o scope — e a conexão com o banco — é descartado antes do app começar a atender requisições.

---

## Desenvolvimento Local

A senha **não deve ser commitada**. Configure-a uma vez via `dotnet user-secrets`:

```bash
cd src/backend
dotnet user-secrets set "SeedAdmin:Senha" "SuaSenhaForte@2026"
```

O `user-secrets` armazena em `~/.microsoft/usersecrets/<project-id>/secrets.json`, fora do repositório. O .NET carrega automaticamente por cima do `appsettings.Development.json`.

Os demais dados do admin ficam em `appsettings.Development.json`:

```json
"SeedAdmin": {
  "Email": "admin@agentesdaalegria.local",
  "Senha": "",
  "NomeCompleto": "Administrador Master",
  "Apelido": "Admin",
  "Telefone": "00000000000"
}
```

Após configurar a senha, basta subir a aplicação normalmente (`dotnet run`). O admin é criado na primeira inicialização.

---

## Produção

O seeder não roda em produção. Para criar o primeiro admin em produção, a abordagem recomendada é subir temporariamente a aplicação em modo Development com variáveis de ambiente, ou criar um endpoint administrativo protegido separado.

Se necessário usar o seeder fora de Development, as credenciais devem ser injetadas via variáveis de ambiente — o .NET usa `__` como separador de seção:

```bash
ASPNETCORE_ENVIRONMENT=Development
SeedAdmin__Email=admin@suaong.org
SeedAdmin__Senha=SenhaForte@2026
SeedAdmin__NomeCompleto=Administrador
SeedAdmin__Apelido=Admin
SeedAdmin__Telefone=11999999999
```

### Fluxo recomendado para o primeiro deploy

1. Suba a aplicação em Development com as variáveis definidas — o admin é criado automaticamente.
2. Faça login com as credenciais configuradas.
3. Crie os demais usuários pelo sistema.
4. Mude o ambiente para Production e remova as variáveis `SeedAdmin__*` — o seeder passa a ser no-op permanentemente.
