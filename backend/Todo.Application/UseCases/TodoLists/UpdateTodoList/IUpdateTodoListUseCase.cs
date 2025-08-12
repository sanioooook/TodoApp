namespace Todo.Application.UseCases.TodoLists.UpdateTodoList;

public interface IUpdateTodoListUseCase
{
    /// <summary>
    /// Handles the asynchronous.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="ct">The <see cref="CancellationToken"/>.</param>
    /// <returns></returns>
    Task<UpdateTodoListResult> HandleAsync(UpdateTodoListCommand command, CancellationToken ct);
}