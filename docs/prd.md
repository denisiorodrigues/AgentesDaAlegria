# Agentes da Alegria — PRD (Product Requirements Document)

**Versão:** 1.0  
**Data:** 2026-05-01  
**Status:** Em revisão  
**Autor:** Equipe Agentes da Alegria

---

## 1. Visão do Produto

**Agentes da Alegria** é uma aplicação web open source para gestão de voluntários e eventos de ONGs, substituindo planilhas Excel e grupos de WhatsApp por um sistema centralizado, acessível por qualquer dispositivo.

Cada ONG publica sua própria instância independente — sem custos de licença, com isolamento total de dados entre organizações.

---

## 2. Problema

ONGs que trabalham com voluntários enfrentam:

- Controle de inscrições feito manualmente em planilhas
- Comunicação dispersa em grupos de WhatsApp
- Sem histórico centralizado de participação dos voluntários
- Controle de presença em papel ou planilhas locais
- Sem notificações automáticas para voluntários

---

## 3. Objetivos

| Objetivo | Métrica de sucesso |
|---|---|
| Centralizar inscrições em eventos | 100% das inscrições via sistema |
| Automatizar fila de espera | Promoção automática sem intervenção manual |
| Registrar histórico de voluntários | Histórico disponível para todos os perfis autorizados |
| Notificar voluntários automaticamente | Notificações por e-mail e push em todos os gatilhos mapeados |
| Facilitar controle de presença | Registro digital por evento pelo coordenador |

---

## 4. Público-alvo

### 4.1 Personas

**Admin (gestor da ONG)**
- Responsável pela plataforma
- Cria eventos, gerencia voluntários, visualiza relatórios
- Frequência de uso: diária / semanal

**Coordenador de Evento**
- Gerencia um evento específico ao qual foi designado
- Registra presença, publica avisos, monitora inscritos e fila de espera
- Frequência de uso: durante o período do evento

**Líder de Equipe**
- Auxilia o coordenador dentro de um evento
- Gerencia um subgrupo de voluntários
- Frequência de uso: durante o período do evento

**Voluntário**
- Se inscreve em eventos, acompanha seu histórico, recebe notificações
- Frequência de uso: semanal / por evento

---

## 5. Funcionalidades

### Épico 1 — Autenticação e Acesso

| ID | História de usuário | Critério de aceite |
|---|---|---|
| AU-01 | Como voluntário, quero me cadastrar na plataforma para ter acesso aos eventos | Cadastro com e-mail, senha e dados pessoais; confirmação por e-mail enviada |
| AU-02 | Como usuário, quero fazer login com e-mail e senha | JWT emitido com expiração; refresh token com rotação |
| AU-03 | Como usuário, quero recuperar minha senha por e-mail | Link de redefinição enviado; expira em 1 hora |
| AU-04 | Como Admin, quero cadastrar manualmente um voluntário | Admin cria conta com dados completos; e-mail de boas-vindas enviado |
| AU-05 | Como sistema, devo controlar acesso por perfil | Perfis: `Admin`, `CoordenadorEvento`, `LiderEquipe`, `Voluntario`; rotas protegidas por perfil |

### Épico 2 — Gestão de Voluntários

| ID | História de usuário | Critério de aceite |
|---|---|---|
| VO-01 | Como voluntário, quero editar meu perfil | Campos editáveis: apelido, telefone, Instagram, endereço |
| VO-02 | Como Admin, quero listar todos os voluntários | Listagem com filtros por nome, perfil e status; paginada |
| VO-03 | Como Admin, quero visualizar o histórico de um voluntário | Exibe eventos participados, presenças e data do último evento |
| VO-04 | Como Admin, quero alterar o perfil de um voluntário | Perfil atualizado imediatamente; log de alteração registrado |
| VO-05 | Como voluntário, quero visualizar meu histórico de participações | Lista de eventos com status de presença e datas |

### Épico 3 — Gestão de Eventos

| ID | História de usuário | Critério de aceite |
|---|---|---|
| EV-01 | Como Admin, quero criar um evento | Campos obrigatórios: nome, data, hora início/fim, local, tipo, total de vagas |
| EV-02 | Como Admin, quero editar um evento | Edição disponível até o início do evento; voluntários inscritos notificados de alterações relevantes |
| EV-03 | Como Admin, quero designar um coordenador para um evento | Coordenador recebe notificação da designação |
| EV-04 | Como qualquer usuário autenticado, quero ver a lista de eventos disponíveis | Exibe eventos futuros com vagas disponíveis e em fila de espera |
| EV-05 | Como qualquer usuário, quero ver os detalhes de um evento | Exibe todos os dados, número de vagas restantes e status da minha inscrição |
| EV-06 | Como Admin, quero configurar o prazo de cancelamento por evento | Prazo padrão: 1 dia antes; configurável no cadastro do evento |

### Épico 4 — Inscrições e Fila de Espera

| ID | História de usuário | Critério de aceite |
|---|---|---|
| IN-01 | Como voluntário, quero me inscrever em um evento | Inscrição confirmada se há vagas; notificação enviada por e-mail e push |
| IN-02 | Como voluntário, quero entrar na fila de espera quando não há vagas | Posição na fila exibida; notificação de entrada na fila enviada |
| IN-03 | Como sistema, devo promover automaticamente o primeiro da fila quando uma vaga abre | Promoção imediata; notificação enviada ao voluntário promovido |
| IN-04 | Como voluntário, quero cancelar minha inscrição dentro do prazo | Cancelamento permitido até o prazo configurado; notificação de confirmação enviada |
| IN-05 | Como coordenador, quero ver a lista de inscritos e a fila de espera do meu evento | Lista ordenada com nome, status e data de inscrição |

### Épico 5 — Controle de Presença

| ID | História de usuário | Critério de aceite |
|---|---|---|
| PR-01 | Como coordenador, quero registrar presença dos voluntários no meu evento | Checklist com todos os inscritos confirmados; registro salvo com data/hora e autor |
| PR-02 | Como coordenador, quero editar um registro de presença já salvo | Edição disponível até 24h após o evento; log de alteração registrado |
| PR-03 | Como Admin, quero ver o relatório de presença de qualquer evento | Exibe lista completa com presença/ausência e percentual de comparecimento |

### Épico 6 — Mural de Avisos

| ID | História de usuário | Critério de aceite |
|---|---|---|
| AV-01 | Como coordenador, quero publicar um aviso no mural do meu evento | Aviso publicado e visível para todos os inscritos; notificação enviada |
| AV-02 | Como voluntário inscrito, quero visualizar os avisos do evento | Avisos listados em ordem cronológica decrescente |
| AV-03 | Como coordenador, quero excluir um aviso publicado | Aviso removido imediatamente do mural |

### Épico 7 — Notificações

| ID | Gatilho | Canal |
|---|---|---|
| NO-01 | Inscrição confirmada | E-mail + Push |
| NO-02 | Entrada na fila de espera | E-mail + Push |
| NO-03 | Promoção da fila para vaga confirmada | E-mail + Push |
| NO-04 | Cancelamento de inscrição | E-mail + Push |
| NO-05 | Lembrete 2 dias antes do evento | E-mail + Push |
| NO-06 | Novo aviso publicado no mural | E-mail + Push |
| NO-07 | Designação como coordenador de evento | E-mail |

**Requisitos técnicos:**
- Voluntário deve aceitar permissão de push notification no navegador
- Suporte: Chrome, Firefox, Safari 16+
- Provedor de e-mail: Resend (ou SMTP genérico configurável)

### Épico 8 — Compartilhamento de Eventos

| ID | História de usuário | Critério de aceite |
|---|---|---|
| CO-01 | Como qualquer usuário, quero compartilhar um evento via WhatsApp | Botão gera link `wa.me` com mensagem e URL do evento pré-formatados |
| CO-02 | Como Admin, quero gerar um link público de um evento | Link acessível sem login; exibe nome, data, local e botão de inscrição (redireciona para login se necessário) |

### Épico 9 — Relatórios e Exportações

| ID | Relatório | Formato | Quem acessa |
|---|---|---|---|
| RE-01 | Lista de presença por evento | PDF imprimível | Admin, Coordenador |
| RE-02 | Base de voluntários | Excel (.xlsx) | Admin |
| RE-03 | Dashboard geral | Gráficos na tela | Admin |

**Dashboard geral inclui:**
- Total de eventos realizados
- Total de voluntários cadastrados
- Total de presenças registradas
- Eventos futuros com vagas em aberto

---

## 6. Requisitos Não-Funcionais

| Requisito | Descrição |
|---|---|
| Responsividade | Interface funcional em qualquer tamanho de tela (desktop, tablet, celular) |
| Plataforma | Web (navegador); sem app nativo necessário |
| Isolamento de dados | Cada ONG opera em instância própria; zero compartilhamento entre organizações |
| Autenticação | JWT com refresh token rotativo; senhas com hash (ASP.NET Core Identity) |
| Performance | Listagens paginadas; respostas da API < 500ms em condições normais |
| Disponibilidade | Hospedagem em Railway ou Render (plano básico); sem SLA formal no escopo inicial |
| Implantação | `docker compose up -d` suficiente para publicar a aplicação completa |
| Open source | Código público; sem dependências de serviços pagos obrigatórios |

---

## 7. Fora do Escopo (v1)

- Multi-tenancy (uma instância para múltiplas ONGs)
- Notificações via WhatsApp Business API
- Chat em tempo real
- Pagamentos ou controle financeiro
- App mobile nativo (iOS / Android)
- Aprovação manual de novos voluntários

---

## 8. Evolução Futura (backlog de ideias)

- Integração com WhatsApp Business API para notificações automáticas
- App mobile nativo
- Multi-tenancy
- Integração com Google Calendar
- Emissão de certificados de voluntariado

---

## 9. Stack Técnica

| Camada | Tecnologia |
|---|---|
| Backend | ASP.NET Core Web API — .NET 8 |
| Frontend | React 18 + Vite |
| Estilização | Tailwind CSS + shadcn/ui |
| Banco de dados | PostgreSQL 16 |
| ORM | Entity Framework Core 8 |
| Autenticação | ASP.NET Core Identity + JWT |
| E-mail | Resend (ou SMTP genérico) |
| Push Notifications | Web Push API |
| Containerização | Docker + Docker Compose |
| Hosting | Railway ou Render |

---

## 10. Modelo de Dados (entidades principais)

```
Voluntario
  - Id, NomeCompleto, Apelido, DataNascimento, Email, Telefone, Instagram, Endereco
  - DataUltimoEvento
  - Perfil (Admin | LiderEquipe | Voluntario)

Evento
  - Id, Nome, Data, HoraInicio, HoraFim, Local, Tipo, Descricao
  - TotalVagas, HorasLimiteCancelamento
  - CoordenadorId (FK → Voluntario)

Inscricao
  - Id, VoluntarioId, EventoId, InscritoEm
  - Status (Confirmado | FilaEspera | Cancelado)
  - PosicaoFilaEspera

Presenca
  - Id, VoluntarioId, EventoId, Presente (bool), RegistradoEm, RegistradoPor

Aviso
  - Id, EventoId, AutorId, Titulo, Corpo, CriadoEm

Notificacao
  - Id, VoluntarioId, Tipo, Titulo, Corpo, EnviadoEm, Lido

AssinaturaPush
  - Id, VoluntarioId, Endpoint, P256dh, Auth
```

---

*Status: Rascunho inicial — baseado em project-definition.md e architecture.md*