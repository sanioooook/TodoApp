namespace Todo.Application.UseCases.TodoLists.UpdateTodoList;

using Common;
using FluentResults;
using Interfaces;

public class UpdateTodoListUseCase(IListRepository repository) : IUpdateTodoListUseCase
{
    /// <inheritdoc />
    public async Task<Result> HandleAsync(UpdateTodoListCommand command, CancellationToken ct)
    {
        var list = await repository.GetByIdAsync(command.Id, ct);
        if (list == null)
            return Result.Fail(Errors.NotFound("TodoList", command.Id));

        if (list.OwnerId != command.CurrentUserId && !list.Shares.Any(x => x.UserId.Equals(command.CurrentUserId)))
            return Result.Fail(Errors.Unauthorized("Only owner or linked user with TodoList can update TodoList"));

        list.Title = command.Title;
        list.UpdatedAt = DateTime.UtcNow;

        await repository.UpdateAsync(list, ct);
        return Result.Ok();
    }
}