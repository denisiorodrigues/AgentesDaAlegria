# Gerenciamento no GitHub Projects — Agentes da Alegria

## Conceitos básicos

### O que é uma Issue?

Uma **issue** é uma **tarefa** registrada no GitHub. Cada funcionalidade do projeto vira uma issue com:

- **Título** — o que precisa ser feito
- **Descrição** — detalhes e critérios de aceite
- **Status** — Todo / In Progress / Done
- **Responsável** — quem vai implementar
- **Label** — categoria/épico ao qual pertence
- **Milestone** — fase/entrega à qual pertence

Exemplo:
> **Issue #1** — `[AU-01] Cadastro de voluntário na plataforma`

---

### O que é um Milestone?

Um **milestone** é um agrupador de issues — representa uma fase ou entrega do projeto.

Cada **épico** (conjunto de funcionalidades relacionadas) vira um milestone:

```
Milestone: Épico 1 — Autenticação e Acesso
  ├── Issue: [AU-01] Cadastro de voluntário
  ├── Issue: [AU-02] Login com e-mail e senha
  ├── Issue: [AU-03] Recuperação de senha
  ├── Issue: [AU-04] Cadastro manual pelo Admin
  └── Issue: [AU-05] Controle de acesso por perfil
```

Quando todas as issues do milestone estiverem em **Done**, o épico está concluído.

---

## Estrutura do board

**Board:** [Agentes da Alegria — Backlog](https://github.com/users/denisiorodrigues/projects/6)

### Colunas de Status (Kanban)

| Coluna | Significado |
|---|---|
| **Todo** | Issue criada, ainda não iniciada |
| **In Progress** | Em desenvolvimento |
| **Done** | Concluída |

### Campos disponíveis no board

| Campo | Uso |
|---|---|
| Title | Nome da issue |
| Assignees | Responsável pela tarefa |
| Status | Todo / In Progress / Done |
| Labels | Categoria/épico e camada (`backend` / `frontend`) |
| Linked pull requests | PR vinculado à issue |
| Milestone | Épico ao qual pertence |
| Repository | Repositório de origem |
| Reviewers | Quem revisou o PR |
| Parent issue | Issue pai (para sub-tasks) |
| Sub-issues progress | Progresso das sub-tasks |

---

## Labels por camada

Cada issue recebe **duas categorias de label**:

1. **Épico** — identifica o grupo funcional (ex.: `autenticacao`, `eventos`)
2. **Camada** — indica onde a implementação acontece:

| Label | Significado |
|---|---|
| `backend` | Implementação na API / servidor |
| `frontend` | Implementação na interface / cliente |

> Uma mesma funcionalidade do PRD gera **uma issue** (não duas). A label de camada indica quem é responsável. Se a funcionalidade exige trabalho nas duas camadas, ambas as labels podem ser aplicadas na mesma issue ou a issue pode ser dividida em sub-tasks.

**Por que não criar milestones separados por camada?** Evita duplicar épicos e mantém a rastreabilidade por fase de entrega — todos os itens de um épico ficam no mesmo milestone independentemente da camada.

---

## Épicos e Milestones

| Milestone | Prefixo das issues | Total |
|---|---|---|
| Épico 1 — Autenticação e Acesso | `AU-xx` | 5 |
| Épico 2 — Gestão de Voluntários | `VO-xx` | 5 |
| Épico 3 — Gestão de Eventos | `EV-xx` | 6 |
| Épico 4 — Inscrições e Fila de Espera | `IN-xx` | 5 |
| Épico 5 — Controle de Presença | `PR-xx` | 3 |
| Épico 6 — Mural de Avisos | `AV-xx` | 3 |
| Épico 7 — Notificações | `NO-xx` | 7 |
| Épico 8 — Compartilhamento de Eventos | `CO-xx` | 2 |
| Épico 9 — Relatórios e Exportações | `RE-xx` | 3 |

---

## Convenção de nomenclatura

Cada issue segue o padrão `[PREFIXO-NÚMERO] Descrição da funcionalidade`.

| Prefixo | Épico |
|---|---|
| `AU` | Autenticação |
| `VO` | Voluntários |
| `EV` | Eventos |
| `IN` | Inscrições |
| `PR` | Presença |
| `AV` | Avisos |
| `NO` | Notificações |
| `CO` | Compartilhamento |
| `RE` | Relatórios |

---

## Fluxo de trabalho completo

```
PRD / Definição do produto
        ↓
   Divide em Épicos (grandes blocos de funcionalidade)
        ↓
   Cada Épico vira um Milestone no GitHub
        ↓
   Cada funcionalidade vira uma Issue dentro do Milestone
   (com label de épico + label de camada: backend ou frontend)
        ↓
   Issue entra no board como "Todo"
        ↓
   Dev pega a issue → cria uma branch a partir da develop correspondente:

   [backend]                        [frontend]
   feature/au-01-cadastro-voluntario    feature/au-01-cadastro-voluntario
        ↓                                        ↓
   merge → develop/backend              merge → develop/frontend

        ↓
   PR usa "Closes #N" no corpo para fechar a issue automaticamente no merge
        ↓
   PR é revisado e aprovado → merge na develop da camada
        ↓
   Issue vai para "Done" no board
```

### Padrão de nome de branch

```
feature/<prefixo-número>-<descricao-curta>

Exemplos:
  feature/au-01-cadastro-voluntario
  feature/ev-02-editar-evento
  feature/no-03-notificacao-inscricao
```

---

## Resumo em tabela

| Conceito | Analogia simples |
|---|---|
| **Issue** | Tarefa individual num quadro de tarefas |
| **Milestone** | Fase/entrega que agrupa várias tarefas |
| **Board (Kanban)** | Quadro com colunas Todo / In Progress / Done |
| **Label** | Etiqueta colorida para categorizar a tarefa |
| **Pull Request** | Pedido para aceitar o código novo no projeto |

---

*A rastreabilidade é o objetivo central: cada linha de código pode ser vinculada à issue que a motivou, ao PR que a revisou e ao milestone que a entregou.*
