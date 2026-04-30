# Agentes da Alegria — Definição do Projeto

## Visão Geral

**Nome:** Agentes da Alegria  
**Tipo:** Aplicação web responsiva (desktop e mobile)  
**Modelo de distribuição:** Cada ONG faz sua própria publicação isolada (não é multi-tenant)  
**Custo:** Gratuito / open source  

**Problema resolvido:** Substituir o gerenciamento manual via planilhas Excel e grupos de WhatsApp por um sistema centralizado de gestão de voluntários e eventos.

---

## Perfis de Usuário

| Perfil | Escopo | Descrição |
|---|---|---|
| **Admin** | Global | Gerencia toda a plataforma da ONG: cria eventos, cadastra coordenadores, acessa relatórios |
| **Coordenador de Evento** | Por evento | Gerencia apenas o evento ao qual foi designado (presenças, avisos, fila de espera) |
| **Líder de Equipe** | Por evento | Auxilia o coordenador, lidera um subgrupo de voluntários dentro do evento |
| **Voluntário** | Global | Visualiza e se inscreve em eventos, acompanha seu histórico |

### Regras de acesso
- Somente o **Admin** pode criar eventos e designar coordenadores
- O **Coordenador** só tem acesso ao seu evento designado
- Voluntários podem se **auto-cadastrar** ou serem cadastrados pelo Admin
- Não há processo de aprovação para novos voluntários

---

## Cadastro de Voluntários

### Dados coletados
- Nome completo
- Apelido
- Data de nascimento
- E-mail (usado no login/cadastro)
- Número de telefone
- Instagram
- Endereço

### Histórico do voluntário
- Lista de eventos participados
- Registro de presença por evento
- Data do último evento com presença confirmada
- Total de eventos participados

---

## Gestão de Eventos

### Dados de um evento
- Nome
- Data e horário
- Local
- Tipo / categoria
- Descrição e instruções para voluntários
- Número total de vagas
- Prazo limite para cancelamento de inscrição (customizável por evento, padrão: 1 dia antes)

### Inscrição
- Voluntários se inscrevem por conta própria
- Ao atingir o limite de vagas, os próximos entram em **fila de espera** automaticamente
- Quando uma vaga abre (cancelamento), o primeiro da fila é promovido automaticamente

### Cancelamento de inscrição
- Permitido pelo próprio voluntário
- Prazo padrão: até 1 dia antes do evento
- Prazo configurável por evento pelo Admin

### Controle de presença
- Coordenador do evento registra presença/ausência de cada voluntário
- Registro fica salvo no histórico do voluntário

---

## Comunicação

### Mural de avisos
- Cada evento tem um mural interno para avisos do coordenador
- Voluntários inscritos podem visualizar os avisos

### Compartilhamento de eventos
- Botão para compartilhar evento via WhatsApp (abre app com mensagem e link pré-formatados)
- Link público do evento pode ser gerado para divulgação

### Notificações
- **E-mail** e **push notification no navegador**
- Exemplos de gatilhos:
  - Inscrição confirmada no evento
  - Entrada na fila de espera
  - Promoção da fila de espera para vaga confirmada
  - Lembrete próximo ao evento (ex: 2 dias antes)
  - Cancelamento de inscrição confirmado
  - Aviso publicado no mural do evento

### Integração com WhatsApp (notificações)
- **Fora do escopo inicial** (WhatsApp Business API tem custo e burocracia elevados para ONGs)
- Previsto como **evolução futura**
- Por ora, comunicação direta pelo WhatsApp é feita manualmente pelos coordenadores

---

## Relatórios e Exportações

| Recurso | Descrição |
|---|---|
| Lista de presença | Imprimível por evento |
| Exportação de voluntários | Exportar base de voluntários para Excel |
| Dashboard geral | Totais: eventos realizados, voluntários cadastrados, presenças, etc. |

---

## Requisitos Não-Funcionais

- **Responsividade:** funciona em qualquer tamanho de tela (desktop, tablet, celular)
- **Plataforma:** web (navegador), sem necessidade de app nativo
- **Isolamento:** cada ONG publica sua própria instância independente
- **Tecnologia:** a definir (sem restrições específicas do cliente)
- **Prazo:** sem deadline definido
- **Custo:** solução gratuita / open source

---

## Fora do Escopo (por ora)

- Multi-tenancy (uma instância para múltiplas ONGs)
- Notificações via WhatsApp (API)
- Chat em tempo real (substituído por mural de avisos + WhatsApp manual)
- Pagamentos ou controle financeiro

---

## Evolução Futura (backlog de ideias)

- Integração com WhatsApp Business API para notificações automáticas
- App mobile nativo (iOS / Android)
- Multi-tenancy para centralizar múltiplas ONGs em uma instância
- Integração com calendários (Google Calendar, etc.)
- Emissão de certificados de voluntariado

---

*Documento gerado em: 2026-04-11*  
*Status: Rascunho inicial — baseado em entrevista com o cliente*
