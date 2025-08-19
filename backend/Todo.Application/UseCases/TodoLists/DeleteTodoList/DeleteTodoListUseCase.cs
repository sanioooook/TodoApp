namespace Todo.Application.UseCases.TodoLists.DeleteTodoList;

using Common;
using FluentResults;
using Interfaces;

public class DeleteTodoListUseCase(IListRepository repository) : IDeleteTodoListUseCase
{
    /// <inheritdoc />
    public async Task<Result> HandleAsync(DeleteTodoListCommand command, CancellationToken ct)
    {
        var list = await repository.GetByIdAsync(command.Id, ct);
        if (list == null)
            return Result.Fail(Errors.NotFound("TodoList", command.Id));

        if (list.OwnerId != command.CurrentUserId)
            return Result.Fail(Errors.Unauthorized("TodoList can't be deleted. Only owner can delete"));

        await repository.DeleteAsync(list, ct);
        return Result.Ok();
    }
}