namespace Todo.Application.UseCases.TodoLists.GetTodoList;

using FluentResults;
using Models;

public interface IGetTodoListUseCase
{
    /// <summary>
    /// Handles the asynchronous.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="ct">The <see cref="CancellationToken"/>.</param>
    /// <returns></returns>
    Task<Result<TodoListDto>> HandleAsync(GetTodoListQuery query, CancellationToken ct);

}