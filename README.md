# Agentes da Alegria

Sistema web de gestão de voluntários e eventos para ONGs — substitui planilhas Excel e grupos de WhatsApp por uma plataforma centralizada, responsiva e gratuita.

> Detalhes técnicos: [`architecture.md`](./architecture.md)  
> Definição do produto: [`project-definition.md`](./project-definition.md)

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

### 2. Aplicar as migrations

```bash
cd backend
dotnet ef database update
```

### 3. Iniciar o backend

```bash
cd backend
dotnet run
```

A API estará disponível em `https://localhost:5001`.

---

## Rodar os testes

```bash
dotnet test
```

---

## Estrutura do projeto

```
AgentesDaAlegria/
├── backend/       # ASP.NET Core Web API (.NET 10)
├── tests/         # Testes de unidade (xUnit)
├── docker-compose.yml
├── architecture.md
└── project-definition.md
```
