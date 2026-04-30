# Agentes da Alegria — Arquitetura e Stack Tecnológica

## Stack Definida

| Camada | Tecnologia | Versão recomendada |
|---|---|---|
| Backend | ASP.NET Core Web API (C#) | .NET 8 (LTS) |
| Frontend | React + Vite | React 18 |
| Estilização | Tailwind CSS + shadcn/ui | Tailwind v3 |
| Banco de dados | PostgreSQL | v16 |
| ORM | Entity Framework Core | EF Core 8 |
| Autenticação | ASP.NET Core Identity + JWT | — |
| E-mail | Resend (ou SMTP genérico) | — |
| Push Notifications | Web Push API | — |
| Containerização | Docker + Docker Compose | — |
| Hosting | Railway ou Render | Plano básico ~$5/mês |

---

## Estrutura de Pastas

```
AgentesDaAlegria/
├── backend/                  # ASP.NET Core Web API
│   ├── Controllers/
│   ├── Models/
│   ├── DTOs/
│   ├── Services/
│   ├── Data/                 # DbContext + Migrations
│   ├── Middleware/
│   └── Program.cs
│
├── frontend/                 # React + Vite
│   ├── src/
│   │   ├── components/       # Componentes reutilizáveis
│   │   ├── pages/            # Páginas da aplicação
│   │   ├── hooks/            # Custom hooks
│   │   ├── services/         # Chamadas à API
│   │   ├── store/            # Estado global (Zustand ou Context)
│   │   └── main.tsx
│   └── vite.config.ts
│
├── docker-compose.yml        # Backend + Frontend + PostgreSQL
└── project-definition.md
```

---

## Arquitetura da API (Backend)

### Módulos principais

| Módulo | Responsabilidade |
|---|---|
| `Auth` | Login, registro, refresh token, recuperação de senha |
| `Voluntarios` | CRUD de voluntários, histórico, perfil |
| `Eventos` | CRUD de eventos, vagas, fila de espera |
| `Inscricoes` | Inscrição, cancelamento, fila de espera |
| `Presencas` | Registro de presença por evento |
| `Avisos` | Mural de avisos por evento |
| `Notificacoes` | E-mail e push notifications |
| `Relatorios` | Relatórios, exportação Excel, dashboard |

### Autenticação
- JWT Bearer Token
- Refresh token com rotação
- Perfis: `Admin`, `CoordenadorEvento`, `LiderEquipe`, `Voluntario`

---

## Banco de Dados — Entidades Principais

```
Voluntario
  - Id, NomeCompleto, Apelido, DataNascimento, Email, Telefone, Instagram, Endereco
  - DataUltimoEvento
  - Perfil (global: Admin, LiderEquipe, Voluntario)

Evento
  - Id, Nome, Data, HoraInicio, HoraFim, Local, Tipo, Descricao
  - TotalVagas, HorasLimiteCancelamento
  - CoordenadorId (FK → Voluntario)

Inscricao
  - Id, VoluntarioId, EventoId, InscritoEm, Status (Confirmado, FilaEspera, Cancelado)
  - PosicaoFilaEspera

Presenca
  - Id, VoluntarioId, EventoId, Presente (bool), RegistradoEm, RegistradoPor

Aviso
  - Id, EventoId, AutorId, Titulo, Corpo, CriadoEm

Notificacao
  - Id, VoluntarioId, Tipo, Titulo, Corpo, EnviadoEm, Lido

PushSubscription
  - Id, VoluntarioId, Endpoint, P256dh, Auth
```

---

## Frontend — Páginas

### Área pública
- `/login` — Login
- `/register` — Cadastro de voluntário
- `/events/:id/public` — Página pública do evento (para compartilhar link)

### Área do voluntário
- `/dashboard` — Próximos eventos, inscrições ativas
- `/events` — Listagem de eventos disponíveis
- `/events/:id` — Detalhes e inscrição no evento
- `/profile` — Editar perfil
- `/history` — Histórico de participações

### Área do coordenador
- `/coordinator/events/:id` — Painel do evento (mural, presenças, inscritos)
- `/coordinator/events/:id/attendance` — Controle de presença
- `/coordinator/events/:id/announcements` — Gerenciar avisos

### Área do admin
- `/admin/dashboard` — Dashboard geral
- `/admin/events` — Gerenciar todos os eventos
- `/admin/events/new` — Criar evento
- `/admin/volunteers` — Gerenciar voluntários
- `/admin/reports` — Relatórios e exportações

---

## Deploy com Docker Compose

```yaml
# docker-compose.yml (simplificado)
services:
  db:
    image: postgres:16
    environment:
      POSTGRES_DB: agentesdaalegria
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: ${DB_PASSWORD}

  backend:
    build: ./backend
    depends_on: [db]
    environment:
      ConnectionStrings__Default: "Host=db;Database=agentesdaalegria;..."
      Jwt__Secret: ${JWT_SECRET}

  frontend:
    build: ./frontend
    depends_on: [backend]
    ports:
      - "80:80"
```

Uma ONG publica o projeto com:
```bash
docker compose up -d
```

---

## Notificações

### E-mail (via Resend ou SMTP)
- Confirmação de inscrição
- Entrada na fila de espera
- Promoção da fila de espera para vaga
- Lembrete 2 dias antes do evento
- Novo aviso no mural do evento

### Push Notification (Web Push API)
- Mesmos gatilhos do e-mail
- Voluntário precisa aceitar permissão no navegador
- Funciona em desktop e mobile (Chrome, Firefox, Safari 16+)

---

## Compartilhamento de Eventos (WhatsApp)

Botão no evento gera link no formato:
```
https://wa.me/?text=Venha%20participar%20do%20evento%20X!%0A%0Ahttps://app.ong.org/events/123
```

---

## Relatórios

| Relatório | Formato |
|---|---|
| Lista de presença por evento | PDF (imprimível) |
| Base de voluntários | Excel (.xlsx) |
| Dashboard geral | Gráficos na tela (Chart.js ou Recharts) |

---

## Decisões Técnicas

| Decisão | Escolha | Motivo |
|---|---|---|
| Blazor vs React | React | Menor bundle, melhor UX mobile, dev já conhece |
| SQL Server vs PostgreSQL | PostgreSQL | Gratuito, sem licença |
| Multi-tenant vs instância por ONG | Instância por ONG | Simplicidade, isolamento total de dados |
| Monorepo vs repos separados | Monorepo | Mais fácil de gerenciar para equipe pequena |

---

*Documento gerado em: 2026-04-11*  
*Status: Arquitetura inicial aprovada*
