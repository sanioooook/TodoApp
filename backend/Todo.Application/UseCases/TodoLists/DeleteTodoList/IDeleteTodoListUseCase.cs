namespace Todo.Application.UseCases.TodoLists.DeleteTodoList;

public interface IDeleteTodoListUseCase
{
    /// <summary>
    /// Handles the asynchronous.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="ct">The <see cref="CancellationToken"/>.</param>
    /// <returns></returns>
    Task<DeleteTodoListResult> HandleAsync(DeleteTodoListCommand command, CancellationToken ct);
}