using Todo.Application.Common;

namespace Todo.Application.UseCases.TodoLists.ShareTodoList;

using FluentResults;
using Domain.Entities;
using Interfaces;

public class ShareTodoListUseCase(IListRepository repository) : IShareTodoListUseCase
{
    /// <inheritdoc />
    public async Task<Result> HandleAsync(ShareTodoListCommand command, CancellationToken ct)
    {

        var list = await repository.GetByIdAsync(command.ListId, ct);

        if(list is null)
            return Result.Fail(Errors.NotFound("TodoList", command.ListId));

        if (list.OwnerId.Equals(command.TargetUserId))
            return Result.Fail("TodoList can't be shared with owner");

        if (list.Shares.Any(x => x.UserId.Equals(command.TargetUserId)))
            return Result.Fail(Errors.Conflict($"TodoList has already shared with user {command.TargetUserId}"));

        if (!list.OwnerId.Equals(command.CurrentUserId) && !list.Shares.Any(x => x.UserId.Equals(command.CurrentUserId)))
            return Result.Fail(Errors.Unauthorized("TodoList can't be shared because current user not owner or linked"));

        if (list.Shares.Count >= 3)
            return Result.Fail("TodoList can't be shared. This list already shared 3 times");

        var share = new TodoListShare { TodoListId = command.ListId, UserId = command.TargetUserId };

        await repository.AddShareAsync(share, ct);
        return Result.Ok();
    }
}