namespace Todo.Application.UseCases.TodoLists.CreateTodoList;

public interface ICreateTodoListUseCase
{
    /// <summary>
    /// Handles the asynchronous.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="ct">The <see cref="CancellationToken"/>.</param>
    /// <returns></returns>
    Task<CreateTodoListResult> HandleAsync(CreateTodoListCommand command, CancellationToken ct);
}