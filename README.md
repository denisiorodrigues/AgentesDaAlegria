# Agentes da Alegria

Sistema web de gestão de voluntários e eventos para ONGs — substitui planilhas Excel e grupos de WhatsApp por uma plataforma centralizada, responsiva e gratuita.

> Detalhes técnicos: [`architecture.md`](./architecture.md)  
> Definição do produto: [`project-definition.md`](./project-definition.md)  
> Requisitos do produto: [`prd.md`](./prd.md)

---

## Funcionalidades principais

- **Gestão de eventos** — criação, edição e divulgação com link público para WhatsApp
- **Inscrições com fila de espera** — promoção automática quando uma vaga é liberada
- **Controle de presença** — registro por evento com histórico por voluntário
- **Mural de avisos** — comunicados do coordenador para os inscritos no evento
- **Notificações** — e-mail e push notification para inscrição, fila, lembretes e avisos
- **Relatórios** — lista de presença (PDF), exportação de voluntários (Excel) e dashboard

---

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

---

## Como iniciar

### 1. Subir o banco de dados

```bash
docker compose up -d
```

Isso inicia dois containers:

| Container | Porta | Descrição |
|---|---|---|
| PostgreSQL 16 | `5432` | Banco de dados |
| Mailpit | `1025` (SMTP) / `8025` (UI) | Interceptador de e-mails para desenvolvimento |

Acesse a caixa de entrada de desenvolvimento em `http://localhost:8025`.

### 2. Configurar os segredos de desenvolvimento

O projeto usa [.NET User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) para manter credenciais fora do código-fonte. Execute os comandos abaixo dentro da pasta `backend/`:

```bash
cd backend

# String de conexão com o banco
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Database=agentesdaalegria;Username=postgres;Password=postgres"

# Chave secreta para assinatura dos tokens JWT — substitua pelo valor gerado abaixo
dotnet user-secrets set "Jwt:Chave" "<chave-gerada>"
```

Para gerar uma chave segura, use um dos comandos antes de rodar o `user-secrets set`:

```bash
# Linux / Mac
openssl rand -base64 48
```

```powershell
# Windows (PowerShell)
[Convert]::ToBase64String((1..48 | ForEach-Object { Get-Random -Max 256 }))
```

> Os segredos ficam armazenados localmente em `~/.microsoft/usersecrets/` e **nunca** são commitados no repositório.

### 3. Aplicar as migrations

```bash
cd backend
dotnet ef database update
```

### 4. Iniciar o backend

```bash
cd backend
dotnet run
```

A API estará disponível em `http://localhost:5206`.

---

## Variáveis de ambiente em produção

Em produção, forneça os segredos via variáveis de ambiente (o ASP.NET Core lê automaticamente):

| Variável de ambiente | Descrição |
|---|---|
| `ConnectionStrings__Default` | String de conexão com o PostgreSQL |
| `Jwt__Chave` | Chave secreta de assinatura do JWT (mín. 32 chars) |

> **Atenção:** use `__` (dois underscores) como separador de seção em variáveis de ambiente.

Exemplo com Docker Compose em produção:

```yaml
backend:
  environment:
    ConnectionStrings__Default: "Host=db;Database=agentesdaalegria;Username=postgres;Password=${DB_PASSWORD}"
    Jwt__Chave: "${JWT_SECRET}"
```

---

## Rodar os testes

```bash
dotnet test
```

---

## Documentação interativa (Scalar)

Com o backend rodando em Development, acesse:

```
http://localhost:5206/scalar/v1
```

O Scalar é gerado automaticamente a partir do spec OpenAPI e permite explorar e executar todos os endpoints. Para testar rotas protegidas, faça login em **AU-02 Login**, copie o `accessToken` da resposta e use o botão **Authenticate** no topo da página.

---

## Testar a API com Bruno

O projeto usa [Bruno](https://www.usebruno.com) como cliente HTTP. A coleção fica versionada em `bruno/` e é gerada automaticamente a partir do spec OpenAPI.

### Pré-requisito

- [Bruno](https://www.usebruno.com/downloads) instalado
- [Node.js](https://nodejs.org) instalado (para o script de geração)

### Abrir a coleção

1. Abra o Bruno
2. Clique em **Open Collection** e selecione a pasta `bruno/`
3. Selecione o ambiente **local**

### Atualizar a coleção (após adicionar uma nova controller)

```powershell
# Windows
.\scripts\update-bruno.ps1

# Linux / Mac
./scripts/update-bruno.sh
```

O script faz dois passos:
1. `dotnet build` → gera `openapi/AgentesDaAlegria.API.json` em build-time
2. `node scripts/openapi-to-bruno.js` → converte o spec em arquivos `.bru`

Os endpoints são agrupados por prefixo de rota (`/api/auth/` → pasta `auth/`, `/api/eventos/` → pasta `eventos/`, etc.). Endpoints com `[Authorize]` já saem configurados com `auth: bearer`.

### Variáveis de ambiente (Bruno)

| Variável | Valor padrão | Descrição |
|---|---|---|
| `baseUrl` | `http://localhost:5000` | URL base da API |
| `accessToken` | _(vazio)_ | Token JWT — preencha após fazer login |

> Para fluxos autenticados: faça login em **AU-02 Login**, copie o `accessToken` da resposta e cole na variável `accessToken` do ambiente local.

---

## Fluxo de branches

```
main                        ← código estável, pronto para produção
└── develop/backend         ← desenvolvimento do backend
    ├── feature/auth
    ├── feature/eventos
    └── feature/inscricoes
```

| Branch | Propósito |
|---|---|
| `main` | Código estável, reflete o que está em produção |
| `develop/backend` | Integração contínua das features do backend |
| `feature/*` | Uma branch por funcionalidade, criada a partir de `develop/backend` |

### Comandos do dia a dia

```bash
# Ver em qual branch está
git branch

# Criar uma feature branch
git checkout -b feature/auth

# Quando a feature estiver pronto, voltar e fazer merge
git checkout develop/backend
git merge feature/auth
```

---

## Estrutura do projeto

```
AgentesDaAlegria/
├── backend/       # ASP.NET Core Web API (.NET 10)
├── tests/         # Testes de unidade e integração (xUnit)
├── bruno/         # Coleção Bruno (gerada — não edite manualmente)
├── openapi/       # Spec OpenAPI gerado em build-time
├── scripts/       # Scripts auxiliares (update-bruno, etc.)
├── docker-compose.yml
├── architecture.md
├── project-definition.md
└── prd.md
```