# TaskFlow — Gerenciador de Tarefas via Console

Aplicação de console em **.NET 10** para gerenciamento de uma lista de tarefas
(To-Do List) em memória, desenvolvida como exercício prático da disciplina
**Garantia da Qualidade de Software** (Gestão e Qualidade de Software).

O projeto foi construído com foco em qualidade: código organizado em camadas
(domínio e serviços), testes unitários com **xUnit** e histórico de commits no
padrão **Conventional Commits**.

---

## Estrutura do projeto

```
taskflow-console/
├── TaskFlow.sln                     # Solution (.NET 10)
├── TaskFlow.Console/                # Aplicação de console
│   ├── Program.cs                   # Menu interativo (laço principal)
│   ├── Domain/
│   │   └── TaskItem.cs              # Entidade de domínio (tarefa)
│   └── Services/
│       └── TaskManager.cs           # Gerenciamento das tarefas em memória
└── TaskFlow.Tests/                  # Testes unitários (xUnit)
    └── TaskManagerTests.cs          # Suíte de testes do TaskManager
```

## Funcionalidades

- Adicionar tarefa (com validação de título vazio/nulo);
- Listar tarefas com status (pendente/concluída) e resumo por contagem;
- Concluir ou reabrir tarefa (alternância de status);
- Buscar tarefa por `Id` (`Guid`) — retorna `null` de forma graciosa quando o
  `Id` não existe (sem `KeyNotFoundException`);
- Remover tarefa;
- Menu interativo com tratamento de entradas inválidas.

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/pt-br/download/dotnet/10.0) instalado
  (verifique com `dotnet --version`);
- Git.

## Como compilar

Na raiz do repositório:

```bash
dotnet build
```

> O comando compila a solution inteira (aplicação de console + testes).

## Como executar a aplicação

```bash
dotnet run --project TaskFlow.Console
```

O menu interativo será exibido:

```
==========================================
      TaskFlow - Gerenciador de Tarefas
==========================================
 1. Adicionar tarefa
 2. Listar tarefas
 3. Concluir / reabrir tarefa
 4. Buscar tarefa por Id
 5. Remover tarefa
 0. Sair
==========================================
```

> As tarefas são mantidas **em memória**: ao encerrar a aplicação, a lista é descartada.

## Como executar os testes unitários

Na raiz do repositório:

```bash
dotnet test
```

Para ver o resultado detalhado de cada teste:

```bash
dotnet test --logger "console;verbosity=detailed"
```

A suíte `TaskManagerTests` cobre, entre outros cenários:

- adição de tarefas válidas (título, `Id` gerado, status inicial pendente);
- rejeição de títulos `null`, vazios ou somente espaços (`ArgumentException`);
- listagem e contagem de tarefas;
- busca por `Id` existente e inexistente (regressão do `KeyNotFoundException`);
- alternância de status (concluir/reabrir) e remoção.

## Convenção de commits

O histórico segue o padrão [Conventional Commits](https://www.conventionalcommits.org/pt-br/):
`tipo(escopo): descrição curta`.

| Tipo       | Uso                                          |
|------------|----------------------------------------------|
| `chore`    | Configuração inicial, tooling, dependências  |
| `feat`     | Nova funcionalidade                          |
| `test`     | Adição/ajuste de testes                      |
| `fix`      | Correção de bug                              |
| `refactor` | Refatoração sem mudança de comportamento     |
| `docs`     | Documentação                                 |

Para inspecionar o histórico:

```bash
git log --oneline --graph
```

## Autor

**Pietro Cardoso** — [pietrocardoso2812@gmail.com](mailto:pietrocardoso2812@gmail.com)
