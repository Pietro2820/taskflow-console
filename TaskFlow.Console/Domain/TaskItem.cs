namespace TaskFlow.Console.Domain;

/// <summary>
/// Representa uma tarefa (item de To-Do List) gerenciada pela aplicação TaskFlow.
/// </summary>
public class TaskItem
{
    /// <summary>Identificador único da tarefa.</summary>
    public Guid Id { get; }

    /// <summary>Título (descrição curta) da tarefa.</summary>
    public string Title { get; }

    /// <summary>Indica se a tarefa já foi concluída.</summary>
    public bool IsCompleted { get; private set; }

    /// <summary>Momento em que a tarefa foi criada.</summary>
    public DateTime CreatedAt { get; }

    /// <summary>Momento da última conclusão da tarefa (nulo se pendente).</summary>
    public DateTime? CompletedAt { get; private set; }

    /// <summary>
    /// Cria uma nova tarefa com um <see cref="Guid"/> gerado automaticamente.
    /// </summary>
    /// <param name="title">Título da tarefa. Não pode ser vazio, nulo ou somente espaços.</param>
    /// <exception cref="ArgumentException">Lançada quando o título é inválido.</exception>
    public TaskItem(string title)
        : this(Guid.NewGuid(), title)
    {
    }

    /// <summary>
    /// Cria uma nova tarefa com um identificador explícito.
    /// </summary>
    /// <param name="id">Identificador único da tarefa.</param>
    /// <param name="title">Título da tarefa. Não pode ser vazio, nulo ou somente espaços.</param>
    /// <exception cref="ArgumentException">Lançada quando o título é inválido.</exception>
    public TaskItem(Guid id, string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("O título da tarefa não pode ser vazio ou nulo.", nameof(title));
        }

        Id = id;
        Title = title.Trim();
        IsCompleted = false;
        CreatedAt = DateTime.Now;
        CompletedAt = null;
    }

    /// <summary>
    /// Alterna o status da tarefa: concluída passa a pendente e pendente passa a concluída.
    /// </summary>
    /// <returns><c>true</c> se a tarefa ficou concluída; <c>false</c> se voltou a ficar pendente.</returns>
    public bool ToggleCompletion()
    {
        IsCompleted = !IsCompleted;
        CompletedAt = IsCompleted ? DateTime.Now : null;
        return IsCompleted;
    }

    /// <summary>Representação textual da tarefa no formato usado pelo console.</summary>
    public override string ToString()
    {
        var status = IsCompleted ? "Concluída" : "Pendente ";
        return $"[{status}] {Title} (Id: {Id})";
    }
}
