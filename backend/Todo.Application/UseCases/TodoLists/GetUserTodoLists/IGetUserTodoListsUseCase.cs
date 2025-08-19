namespace Todo.Application.UseCases.TodoLists.GetUserTodoLists;

using FluentResults;
using Models.TodoList;

public interface IGetUserTodoListsUseCase
{
    /// <summary>
    /// Handles the asynchronous.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="ct">The <see cref="CancellationToken"/>.</param>
    /// <returns></returns>
    Task<Result<IEnumerable<TodoListDto>>> HandleAsync(GetUserTodoListsQuery query, CancellationToken ct);

}