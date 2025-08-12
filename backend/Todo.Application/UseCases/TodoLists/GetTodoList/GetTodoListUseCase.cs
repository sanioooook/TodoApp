namespace Todo.Application.UseCases.TodoLists.GetTodoList;

using Enums;
using Interfaces;
using Models;

public class GetTodoListUseCase : IGetTodoListUseCase
{
    private readonly IListRepository _repository;

    public GetTodoListUseCase(IListRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public async Task<GetTodoListResult> HandleAsync(GetTodoListQuery query, CancellationToken ct)
    {
        var list = await _repository.GetByIdAsync(query.ListId, query.UserId, ct);
        if (list is null)
        {
            return new GetTodoListResult { HaveResult = false, CodeResult = ResultCode.NotFound, Message = "TodoList not found" };
        }

        return new GetTodoListResult
        {
            TodoListDto = new TodoListDto
            {
                Id = list.Id,
                Title = list.Title,
                CreatedAt = list.CreatedAt,
                UpdatedAt = list.UpdatedAt,
                OwnerId = list.OwnerId,
                SharedWithUsers = list.Shares.Select(x => new TodoListShareDto { UserId = x.UserId }),
            },
            HaveResult = true,
            CodeResult = ResultCode.Success,
            Message = string.Empty,
        };
    }
}