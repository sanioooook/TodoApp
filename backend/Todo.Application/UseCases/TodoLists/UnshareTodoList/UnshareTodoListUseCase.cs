namespace Todo.Application.UseCases.TodoLists.UnshareTodoList;

using Common;
using FluentResults;
using Interfaces;

public class UnshareTodoListUseCase(IListRepository repository, IUserRepository userRepository) : IUnshareTodoListUseCase
{
    /// <inheritdoc />
    public async Task<Result> HandleAsync(UnshareTodoListCommand command, CancellationToken ct)
    {
        var list = await repository.GetByIdAsync(command.ListId, ct);
        var user = await userRepository.GetByIdAsync(command.TargetUserId, ct);

        if (user == null)
            return Result.Fail(Errors.NotFound("User", command.TargetUserId));

        if (list == null)
            return Result.Fail(Errors.NotFound("TodoList", command.ListId));

        if (list.OwnerId.Equals(command.TargetUserId))
            return Result.Fail("TodoList can't be unshared of owner");
        
        if (!list.OwnerId.Equals(command.CurrentUserId) && !list.Shares.Any(x => x.UserId.Equals(command.CurrentUserId)))
            return Result.Fail(Errors.Unauthorized("TodoList can't be unshared because current user not owner or linked"));

        var share = list.Shares.First(x => x.UserId == command.TargetUserId);

        await repository.RemoveShareAsync(share, ct);

        return Result.Ok();
    }
}