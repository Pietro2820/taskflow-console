using TaskFlow.Console.Domain;

namespace TaskFlow.Console.Services;

/// <summary>
/// Responsável por gerenciar a lista de tarefas em memória
/// (adicionar, listar, buscar, alternar status e remover).
/// </summary>
public class TaskManager
{
    private readonly Dictionary<Guid, TaskItem> _tasks = new();

    /// <summary>Quantidade de tarefas atualmente gerenciadas.</summary>
    public int Count => _tasks.Count;

    /// <summary>
    /// Adiciona uma nova tarefa à lista.
    /// </summary>
    /// <param name="title">Título da tarefa. Não pode ser vazio, nulo ou somente espaços.</param>
    /// <returns>A tarefa recém-criada.</returns>
    /// <exception cref="ArgumentException">Lançada quando o título é inválido.</exception>
    public TaskItem AddTask(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("O título da tarefa não pode ser vazio ou nulo.", nameof(title));
        }

        var task = new TaskItem(title);
        _tasks.Add(task.Id, task);
        return task;
    }

    /// <summary>
    /// Lista todas as tarefas, na ordem em que foram adicionadas.
    /// </summary>
    public IReadOnlyList<TaskItem> GetAllTasks() =>
        _tasks.Values.OrderBy(t => t.CreatedAt).ToList();

    /// <summary>
    /// Busca uma tarefa pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da tarefa.</param>
    /// <returns>A tarefa correspondente ao <paramref name="id"/>.</returns>
    /// <exception cref="KeyNotFoundException">Lançada quando não existe tarefa com o id informado.</exception>
    public TaskItem GetById(Guid id) => _tasks[id];

    /// <summary>
    /// Alterna o status (pendente/concluída) da tarefa identificada por <paramref name="id"/>.
    /// </summary>
    /// <returns>O novo status da tarefa (<c>true</c> = concluída).</returns>
    public bool ToggleTask(Guid id)
    {
        var task = GetById(id);
        return task.ToggleCompletion();
    }

    /// <summary>
    /// Remove a tarefa identificada por <paramref name="id"/>.
    /// </summary>
    /// <returns><c>true</c> se a tarefa foi removida; <c>false</c> se o id não existia.</returns>
    public bool RemoveTask(Guid id) => _tasks.Remove(id);
}
