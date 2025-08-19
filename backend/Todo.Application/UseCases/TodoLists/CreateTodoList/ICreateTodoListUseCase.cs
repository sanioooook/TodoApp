namespace Todo.Application.UseCases.TodoLists.CreateTodoList;

using FluentResults;
using Models.TodoList;

public interface ICreateTodoListUseCase
{
    /// <summary>
    /// Handles the asynchronous.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="ct">The <see cref="CancellationToken"/>.</param>
    /// <returns></returns>
    Task<Result<TodoListDto>> HandleAsync(CreateTodoListCommand command, CancellationToken ct);
}