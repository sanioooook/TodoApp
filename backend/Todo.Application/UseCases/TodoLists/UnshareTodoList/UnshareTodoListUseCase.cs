namespace Todo.Application.UseCases.TodoLists.UnshareTodoList;

using Enums;
using Interfaces;

public class UnshareTodoListUseCase : IUnshareTodoListUseCase
{
    private readonly IListRepository _repository;
    private readonly IUserRepository _userRepository;

    public UnshareTodoListUseCase(IListRepository repository, IUserRepository userRepository)
    {
        _repository = repository;
        _userRepository = userRepository;
    }

    /// <inheritdoc />
    public async Task<UnshareTodoListResult> HandleAsync(UnshareTodoListCommand command, CancellationToken ct)
    {
        var list = await _repository.GetByIdAsync(command.ListId, command.CurrentUserId, ct);
        var user = await _userRepository.GetByIdAsync(command.TargetUserId, ct);

        if (user == null)
            return new UnshareTodoListResult { Success = false, Message = "Target User not found", CodeResult = ResultCode.NotFound };

        if (list == null)
            return new UnshareTodoListResult { Success = false, Message = "TodoList not found", CodeResult = ResultCode.NotFound };

        if (list.OwnerId.Equals(command.TargetUserId))
            return new UnshareTodoListResult { Success = false, Message = "TodoList can't be unshared of owner", CodeResult = ResultCode.BadRequest };

        if (!list.OwnerId.Equals(command.CurrentUserId) && !list.Shares.Any(x => x.UserId.Equals(command.CurrentUserId)))
            return new UnshareTodoListResult { Success = false, Message = "TodoList can't be unshared because current user not owner or linked", CodeResult = ResultCode.Forbidden };

        var unshare = await _repository.RemoveShareAsync(command.ListId, command.TargetUserId, ct);

        if (!unshare)
            return new UnshareTodoListResult { Success = false, Message = "Error while unsharing TodoList", CodeResult = ResultCode.ServerError };

        return new UnshareTodoListResult { Success = true, Message = "Successfully unshared TodoList", CodeResult = ResultCode.Success };
    }
}