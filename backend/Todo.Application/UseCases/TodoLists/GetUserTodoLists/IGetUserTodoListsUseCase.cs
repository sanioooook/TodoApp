namespace Todo.Application.UseCases.TodoLists.GetUserTodoLists;

public interface IGetUserTodoListsUseCase
{
    /// <summary>
    /// Handles the asynchronous.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="ct">The <see cref="CancellationToken"/>.</param>
    /// <returns></returns>
    Task<IEnumerable<GetUserTodoListsResult>> HandleAsync(GetUserTodoListsQuery query, CancellationToken ct);

}