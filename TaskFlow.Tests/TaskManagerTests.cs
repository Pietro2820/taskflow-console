using TaskFlow.Console.Domain;
using TaskFlow.Console.Services;

namespace TaskFlow.Tests;

/// <summary>
/// Suíte de testes unitários das regras de validação e do comportamento
/// básico do <see cref="TaskManager"/>.
/// </summary>
public class TaskManagerTests
{
    private readonly TaskManager _manager = new();

    // ---------------------------------------------------------------
    // Adição de tarefas válidas
    // ---------------------------------------------------------------

    [Fact]
    public void AddTask_WithValidTitle_ShouldAddTaskToList()
    {
        var task = _manager.AddTask("Estudar para a prova de GQS");

        Assert.NotNull(task);
        Assert.Equal("Estudar para a prova de GQS", task.Title);
        Assert.Equal(1, _manager.Count);
        Assert.Contains(task, _manager.GetAllTasks());
    }

    [Fact]
    public void AddTask_WithValidTitle_ShouldCreatePendingTaskWithGeneratedId()
    {
        var task = _manager.AddTask("Escrever README");

        Assert.NotEqual(Guid.Empty, task.Id);
        Assert.False(task.IsCompleted);
        Assert.Null(task.CompletedAt);
    }

    [Fact]
    public void AddTask_MultipleTimes_ShouldIncreaseCount()
    {
        _manager.AddTask("Tarefa 1");
        _manager.AddTask("Tarefa 2");
        _manager.AddTask("Tarefa 3");

        Assert.Equal(3, _manager.Count);
        Assert.Equal(3, _manager.GetAllTasks().Count);
    }

    [Fact]
    public void AddTask_WithSurroundingWhitespace_ShouldTrimTitle()
    {
        var task = _manager.AddTask("   tarefa com espaços   ");

        Assert.Equal("tarefa com espaços", task.Title);
    }

    // ---------------------------------------------------------------
    // Regras de validação: títulos vazios ou nulos são rejeitados
    // ---------------------------------------------------------------

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   \t  ")]
    public void AddTask_WithEmptyNullOrWhitespaceTitle_ShouldThrowArgumentException(string? title)
    {
        var ex = Assert.Throws<ArgumentException>(() => _manager.AddTask(title!));

        Assert.Equal("title", ex.ParamName);
        Assert.Equal(0, _manager.Count);
    }

    // ---------------------------------------------------------------
    // Listagem, busca e alternância de status
    // ---------------------------------------------------------------

    [Fact]
    public void GetAllTasks_ShouldReturnAllAddedTasks()
    {
        var first = _manager.AddTask("Primeira");
        var second = _manager.AddTask("Segunda");

        var all = _manager.GetAllTasks();

        Assert.Equal(2, all.Count);
        Assert.Equal(first.Id, all[0].Id);
        Assert.Equal(second.Id, all[1].Id);
    }

    [Fact]
    public void GetById_WithExistingId_ShouldReturnTheSameTask()
    {
        var task = _manager.AddTask("Preparar apresentação");

        var found = _manager.GetById(task.Id);

        Assert.NotNull(found);
        Assert.Equal(task.Id, found.Id);
        Assert.Equal("Preparar apresentação", found.Title);
    }

    [Fact]
    public void GetById_WithUnknownId_ShouldThrowKeyNotFoundException()
    {
        // Comportamento atual (bug conhecido): quando o id não existe,
        // o método propaga uma KeyNotFoundException não tratada.
        Assert.Throws<KeyNotFoundException>(() => _manager.GetById(Guid.NewGuid()));
    }

    [Fact]
    public void ToggleTask_OnPendingTask_ShouldMarkItAsCompleted()
    {
        var task = _manager.AddTask("Finalizar lista de exercícios");

        var isCompleted = _manager.ToggleTask(task.Id);

        Assert.True(isCompleted);
        Assert.True(task.IsCompleted);
        Assert.NotNull(task.CompletedAt);
    }

    [Fact]
    public void ToggleTask_Twice_ShouldReturnTaskToPending()
    {
        var task = _manager.AddTask("Revisar commits");

        _manager.ToggleTask(task.Id);
        var isCompleted = _manager.ToggleTask(task.Id);

        Assert.False(isCompleted);
        Assert.False(task.IsCompleted);
        Assert.Null(task.CompletedAt);
    }

    [Fact]
    public void RemoveTask_WithExistingId_ShouldRemoveFromList()
    {
        var task = _manager.AddTask("Tarefa temporária");

        var removed = _manager.RemoveTask(task.Id);

        Assert.True(removed);
        Assert.Equal(0, _manager.Count);
    }

    [Fact]
    public void RemoveTask_WithUnknownId_ShouldReturnFalse()
    {
        Assert.False(_manager.RemoveTask(Guid.NewGuid()));
    }
}
