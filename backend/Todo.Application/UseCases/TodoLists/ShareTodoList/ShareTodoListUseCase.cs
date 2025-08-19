namespace Todo.Application.UseCases.TodoLists.ShareTodoList;

using Domain.Entities;
using Enums;
using Interfaces;

public class ShareTodoListUseCase : IShareTodoListUseCase
{
    private readonly IListRepository _repository;

    public ShareTodoListUseCase(IListRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public async Task<ShareTodoListResult> HandleAsync(ShareTodoListCommand command, CancellationToken ct)
    {

        var list = await _repository.GetByIdAsync(command.ListId, ct);

        if(list is null)
            return new ShareTodoListResult { Success = false, Message = "TodoList not found", CodeResult = ResultCode.NotFound };

        if (list.OwnerId.Equals(command.TargetUserId))
            return new ShareTodoListResult { Success = false, Message = "TodoList can't be shared with owner", CodeResult = ResultCode.BadRequest };

        if (list.Shares.Any(x => x.UserId.Equals(command.TargetUserId)))
            return new ShareTodoListResult { Success = false, Message = $"TodoList has already shared with user {command.TargetUserId}", CodeResult = ResultCode.BadRequest };

        if (!list.OwnerId.Equals(command.CurrentUserId) && !list.Shares.Any(x => x.UserId.Equals(command.CurrentUserId)))
            return new ShareTodoListResult { Success = false, Message = "TodoList can't be shared because current user not owner or linked", CodeResult = ResultCode.Forbidden};

        if (list.Shares.Count >= 3)
            return new ShareTodoListResult { Success = false, Message = "TodoList can't be shared. This list already shared 3 times", CodeResult = ResultCode.BadRequest };

        var share = new TodoListShare { TodoListId = command.ListId, UserId = command.TargetUserId };

        await _repository.AddShareAsync(share, ct);
        return new ShareTodoListResult { Success = true, Message = "Successfully share TodoList", CodeResult = ResultCode.Success};
    }
}