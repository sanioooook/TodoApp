namespace Todo.Application.UseCases.TodoLists.DeleteTodoList;

using Enums;
using Interfaces;

public class DeleteTodoListUseCase : IDeleteTodoListUseCase
{
    private readonly IListRepository _repository;

    public DeleteTodoListUseCase(IListRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public async Task<DeleteTodoListResult> HandleAsync(DeleteTodoListCommand command, CancellationToken ct)
    {
        var list = await _repository.GetByIdAsync(command.Id, command.CurrentUserId, ct);
        if (list == null)
            return new DeleteTodoListResult { Success = false, Message = "TodoList not found", CodeResult = ResultCode.NotFound };

        if (list.OwnerId != command.CurrentUserId)
            return new DeleteTodoListResult
                { Success = false, Message = "TodoList can't be deleted. Only owner can delete", CodeResult = ResultCode.Forbidden };

        var deleted = await _repository.DeleteAsync(command.Id, ct);
        if (!deleted)
            return new DeleteTodoListResult
                { Success = false, Message = "Error while deleting TodoList", CodeResult = ResultCode.ServerError };
        return new DeleteTodoListResult { Success = true, Message = "Successfully deleted TodoList", CodeResult = ResultCode.Success };
    }
}