namespace Todo.Application.UseCases.TodoLists.UnshareTodoList;

public interface IUnshareTodoListUseCase
{
    /// <summary>
    /// Handles the asynchronous.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="ct">The <see cref="CancellationToken"/>.</param>
    /// <returns></returns>
    Task<UnshareTodoListResult> HandleAsync(UnshareTodoListCommand command, CancellationToken ct);
}