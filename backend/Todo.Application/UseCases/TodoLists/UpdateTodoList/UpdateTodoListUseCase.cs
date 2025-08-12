namespace Todo.Application.UseCases.TodoLists.UpdateTodoList;

using Enums;
using Interfaces;

public class UpdateTodoListUseCase : IUpdateTodoListUseCase
{
    private readonly IListRepository _repository;

    public UpdateTodoListUseCase(IListRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public async Task<UpdateTodoListResult> HandleAsync(UpdateTodoListCommand command, CancellationToken ct)
    {
        var list = await _repository.GetByIdAsync(command.Id, command.CurrentUserId, ct);
        if (list == null)
            return new UpdateTodoListResult {Success = false, Message = "TodoList not found", CodeResult = ResultCode.NotFound};

        if (list.OwnerId != command.CurrentUserId && !list.Shares.Any(x => x.UserId.Equals(command.CurrentUserId)))
            return new UpdateTodoListResult { Success = false, Message = "Only owner or linked user with TodoList can update TodoList", CodeResult = ResultCode.Forbidden};

        list.Title = command.Title;
        list.UpdatedAt = DateTime.UtcNow;

        var updated = await _repository.UpdateAsync(list, ct);
        if (!updated)
            return new UpdateTodoListResult { Success = false, Message = "Error wile updating TodoList", CodeResult = ResultCode.ServerError};
        return new UpdateTodoListResult { Success = true, Message = "Successfully update TodoList", CodeResult = ResultCode.Success};
    }
}