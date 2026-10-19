using TaskFlow.Console.Domain;
using TaskFlow.Console.Services;

// ---------------------------------------------------------------------------
// TaskFlow — Gerenciador de Tarefas (Console)
// Ponto de entrada da aplicação: laço interativo de menu sobre o TaskManager.
// ---------------------------------------------------------------------------

var manager = new TaskManager();
var running = true;

Console.WriteLine("Bem-vindo(a) ao TaskFlow!");

while (running)
{
    PrintMenu();
    var option = Console.ReadLine()?.Trim();

    switch (option)
    {
        case "1":
            AddTask();
            break;
        case "2":
            ListTasks();
            break;
        case "3":
            ToggleTaskStatus();
            break;
        case "4":
            SearchTaskById();
            break;
        case "5":
            RemoveTask();
            break;
        case "0":
            running = false;
            WriteSuccess("Obrigado por usar o TaskFlow. Até logo!");
            break;
        case null:
            // Entrada encerrada (EOF): sai do laço sem erro.
            running = false;
            break;
        default:
            WriteError($"Opção inválida: \"{option}\". Escolha um dos números do menu.");
            break;
    }
}

// ----------------------------- Funções locais ------------------------------

void PrintMenu()
{
    if (!Console.IsOutputRedirected)
    {
        Console.Clear();
    }

    Console.WriteLine("==========================================");
    Console.WriteLine("      TaskFlow - Gerenciador de Tarefas   ");
    Console.WriteLine("==========================================");
    Console.WriteLine($" Tarefas na memória: {manager.Count}");
    Console.WriteLine("------------------------------------------");
    Console.WriteLine(" 1. Adicionar tarefa");
    Console.WriteLine(" 2. Listar tarefas");
    Console.WriteLine(" 3. Concluir / reabrir tarefa");
    Console.WriteLine(" 4. Buscar tarefa por Id");
    Console.WriteLine(" 5. Remover tarefa");
    Console.WriteLine(" 0. Sair");
    Console.WriteLine("------------------------------------------");
    Console.Write("Escolha uma opção: ");
}

void AddTask()
{
    Console.Write("Título da nova tarefa: ");
    var title = Console.ReadLine();

    try
    {
        var task = manager.AddTask(title!);
        WriteSuccess($"Tarefa adicionada com sucesso (Id: {task.Id}).");
    }
    catch (ArgumentException)
    {
        WriteError("O título da tarefa não pode ser vazio ou nulo. Tente novamente.");
    }

    WaitToContinue();
}

void ListTasks()
{
    var tasks = manager.GetAllTasks();
    if (tasks.Count == 0)
    {
        WriteError("Nenhuma tarefa cadastrada ainda.");
        WaitToContinue();
        return;
    }

    var pending = tasks.Count(t => !t.IsCompleted);
    Console.WriteLine($" Total: {tasks.Count} | Pendentes: {pending} | Concluídas: {tasks.Count - pending}");
    Console.WriteLine("------------------------------------------");
    for (var i = 0; i < tasks.Count; i++)
    {
        Console.WriteLine($"  #{i + 1} {tasks[i]}");
    }

    WaitToContinue();
}

void ToggleTaskStatus()
{
    var task = SelectTask("alternar o status");
    if (task is not null)
    {
        var newStatus = manager.ToggleTask(task.Id);
        if (newStatus is null)
        {
            WriteError("Tarefa não encontrada.");
        }
        else
        {
            var status = newStatus == true ? "CONCLUÍDA" : "PENDENTE";
            WriteSuccess($"\"{task.Title}\" agora está {status}.");
        }
    }

    WaitToContinue();
}

void SearchTaskById()
{
    Console.Write("Digite o Id (Guid) da tarefa: ");
    var input = Console.ReadLine()?.Trim();

    if (!Guid.TryParse(input, out var id))
    {
        WriteError("Id inválido. Use o formato Guid exibido na listagem (ex.: 9c1b2f3e-4d5a-...).");
        WaitToContinue();
        return;
    }

    // GetById retorna null quando o Id não existe (correção aplicada no TaskManager).
    var task = manager.GetById(id);
    if (task is null)
    {
        WriteError($"Nenhuma tarefa encontrada com o Id {id}.");
    }
    else
    {
        WriteSuccess("Tarefa encontrada:");
        Console.WriteLine($"  Título:     {task.Title}");
        Console.WriteLine($"  Id:         {task.Id}");
        Console.WriteLine($"  Status:     {(task.IsCompleted ? "Concluída" : "Pendente")}");
        Console.WriteLine($"  Criada em:  {task.CreatedAt:dd/MM/yyyy HH:mm:ss}");
        Console.WriteLine($"  Concluída:  {(task.CompletedAt is null ? "-" : task.CompletedAt.Value.ToString("dd/MM/yyyy HH:mm:ss"))}");
    }

    WaitToContinue();
}

void RemoveTask()
{
    var task = SelectTask("remover");
    if (task is not null)
    {
        var removed = manager.RemoveTask(task.Id);
        if (removed)
        {
            WriteSuccess($"Tarefa \"{task.Title}\" removida.");
        }
        else
        {
            WriteError("Tarefa não encontrada.");
        }
    }

    WaitToContinue();
}

/// <summary>
/// Lista as tarefas numeradas e pede ao usuário que escolha uma pelo número.
/// Retorna <c>null</c> se não houver tarefas, se o número for inválido ou se o
/// usuário digitar 0 (cancelar).
/// </summary>
TaskItem? SelectTask(string action)
{
    var tasks = manager.GetAllTasks();
    if (tasks.Count == 0)
    {
        WriteError("Nenhuma tarefa cadastrada ainda.");
        return null;
    }

    Console.WriteLine($"Escolha a tarefa que deseja {action}:");
    for (var i = 0; i < tasks.Count; i++)
    {
        Console.WriteLine($"  #{i + 1} {tasks[i]}");
    }

    Console.Write("Número da tarefa (0 para cancelar): ");
    var input = Console.ReadLine()?.Trim();

    if (input == "0")
    {
        return null;
    }

    if (!int.TryParse(input, out var number) || number < 1 || number > tasks.Count)
    {
        WriteError("Número inválido. Operação cancelada.");
        return null;
    }

    return tasks[number - 1];
}

void WaitToContinue()
{
    Console.WriteLine("------------------------------------------");
    Console.Write("Pressione ENTER para voltar ao menu...");
    Console.ReadLine();
}

void WriteError(string message) => WriteColored($"[ERRO] {message}", ConsoleColor.Red);

void WriteSuccess(string message) => WriteColored($"[OK] {message}", ConsoleColor.Green);

void WriteColored(string message, ConsoleColor color)
{
    if (Console.IsOutputRedirected)
    {
        Console.WriteLine(message);
        return;
    }

    var previous = Console.ForegroundColor;
    Console.ForegroundColor = color;
    Console.WriteLine(message);
    Console.ForegroundColor = previous;
}
