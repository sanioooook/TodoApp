namespace Todo.Application.UseCases.TodoLists.ShareTodoList;

using FluentResults;

public interface IShareTodoListUseCase
{
    /// <summary>
    /// Handles the asynchronous.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="ct">The <see cref="CancellationToken"/>.</param>
    /// <returns></returns>
    Task<Result> HandleAsync(ShareTodoListCommand command, CancellationToken ct);
}