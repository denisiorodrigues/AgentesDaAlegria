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

Isso inicia um container PostgreSQL 16 na porta `5432`.

### 2. Configurar os segredos de desenvolvimento

O projeto usa [.NET User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) para manter credenciais fora do código-fonte. Execute os comandos abaixo dentro da pasta `backend/`:

```bash
cd backend

# String de conexão com o banco
dotnet user-secrets set "ConnectionStrings:Default" \
  "Host=localhost;Port=5432;Database=agentesdaalegria;Username=postgres;Password=postgres"

# Chave secreta para assinatura dos tokens JWT (mínimo 32 caracteres)
dotnet user-secrets set "Jwt:Chave" "sua-chave-secreta-com-no-minimo-32-chars"
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
├── tests/         # Testes de unidade (xUnit)
├── docker-compose.yml
├── architecture.md
├── project-definition.md
└── prd.md
```